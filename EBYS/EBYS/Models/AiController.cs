using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBYS.Controllers;

[Authorize]
[ApiController]
[Route("Ai")]
public class AiController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly IConfiguration _config;

    public AiController(IHttpClientFactory http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public record AiDto(string mode, string prompt, string konu, string baslik, string existingHtml);

    [HttpPost("Compose")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Compose([FromBody] AiDto dto)
    {
        var apiKey = _config["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"];
        var model = _config["gpt-4o-mini"];

        if (string.IsNullOrWhiteSpace(apiKey))
            return BadRequest(new { error = "OpenAI:ApiKey eksik (User Secrets / Environment Variable ile verin)." });

        if (string.IsNullOrWhiteSpace(model))
            return BadRequest(new { error = "OpenAI:Model eksik (örn: gpt-4o-mini)." });

        var instruction = dto.mode switch
        {
            "rewrite" => "Aşağıdaki metni resmi Türkçe yazışma diline çevir. SADECE HTML döndür (markdown yok, açıklama yok).",
            "summarize" => "Aşağıdaki metni resmi bir memo gibi özetle. SADECE HTML döndür (markdown yok, açıklama yok).",
            _ => "Resmi Türkçe yazışma formatında bir belge hazırla. SADECE HTML döndür (markdown yok, açıklama yok)."
        };

        // Kullanıcı promptunu daha zengin yapalım (konu/başlık/mevcut içerik)
        var userPrompt = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(dto.konu)) userPrompt.AppendLine($"Konu: {dto.konu}");
        if (!string.IsNullOrWhiteSpace(dto.baslik)) userPrompt.AppendLine($"Başlık: {dto.baslik}");
        userPrompt.AppendLine();

        if (!string.IsNullOrWhiteSpace(dto.prompt))
        {
            userPrompt.AppendLine("İstek:");
            userPrompt.AppendLine(dto.prompt);
            userPrompt.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(dto.existingHtml))
        {
            userPrompt.AppendLine("Mevcut içerik (HTML):");
            userPrompt.AppendLine(dto.existingHtml);
        }

        var payload = new
        {
            model = model,
            input = new object[]
            {
                new { role = "system", content = instruction },
                new { role = "user", content = userPrompt.ToString() }
            }
        };

        var client = _http.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        using var response = await client.PostAsync(
            "https://api.openai.com/v1/responses",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        );

        var json = await response.Content.ReadAsStringAsync();

        // Hata durumunda OpenAI cevabını döndür (debug için çok faydalı)
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                error = "OpenAI isteği başarısız.",
                status = (int)response.StatusCode,
                body = json
            });
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // ✅ Robust parsing (output_text her zaman gelmez)
        string? html = null;

        // Case 1: direct output_text
        if (root.TryGetProperty("output_text", out var direct))
            html = direct.GetString();

        // Case 2: standard Responses API -> output[].content[].{type:"output_text", text:"..."}
        if (html == null && root.TryGetProperty("output", out var outputArray)
            && outputArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in outputArray.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var contentArray) || contentArray.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var content in contentArray.EnumerateArray())
                {
                    if (content.TryGetProperty("type", out var typeProp)
                        && typeProp.GetString() == "output_text"
                        && content.TryGetProperty("text", out var textProp))
                    {
                        html = textProp.GetString();
                        break;
                    }
                }

                if (html != null) break;
            }
        }

        if (string.IsNullOrWhiteSpace(html))
        {
            return BadRequest(new
            {
                error = "AI cevabı boş veya beklenen formatta değil.",
                body = json
            });
        }

        return Ok(new { html });
    }
}
