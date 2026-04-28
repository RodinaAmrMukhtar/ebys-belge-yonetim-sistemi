#nullable enable

using System;
using System.Threading.Tasks;
using EBYS.Data;
using EBYS.Models;
using EBYS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

// 🔒 ALIASES
using ModelDocument = EBYS.Models.Document;
using PdfDocument = QuestPDF.Fluent.Document;

namespace EBYS.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _email;

        public DocumentController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            EmailService email)
        {
            _context = context;
            _userManager = userManager;
            _email = email;
        }

        private Task<IdentityUser?> CurrentUserAsync()
            => _userManager.GetUserAsync(User);

        // =====================================================
        // CREATE / EDIT (GET)
        // /Document/Create or /Document/Create?id=5
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            var user = await CurrentUserAsync();
            if (user == null) return Unauthorized();

            var profile = await _context.UserProfiles
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            ModelDocument doc;

            if (id.HasValue)
            {
                doc = await _context.Documents.FirstOrDefaultAsync(d =>
                    d.Id == id.Value &&
                    d.CreatedByUserId == user.Id &&
                    d.Durum == "Draft" &&
                    !d.IsDeleted);

                if (doc == null) return NotFound();
            }
            else
            {
                doc = new ModelDocument
                {
                    CreatedByUserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Durum = "Draft",
                    IsDeleted = false
                };
            }

            if (profile != null)
            {
                doc.UreticiBelgesi = profile.FullName;
                doc.UretimYeri = profile.Department?.Name ?? "Birim";
            }

            ViewBag.Users = await _context.UserProfiles
                .Select(u => new
                {
                    IdentityUserId = u.IdentityUserId,
                    FullName = u.FullName
                })
                .ToListAsync();

            return View(doc);
        }

        // =====================================================
        // SAVE DRAFT (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDraft(ModelDocument document)
        {
            var user = await CurrentUserAsync();
            if (user == null) return Unauthorized();

            EnsureDbSafe(document);

            if (document.Id == 0)
            {
                document.CreatedByUserId = user.Id;
                document.Durum = "Draft";
                document.IsDeleted = false;
                document.CreatedAt = DateTime.UtcNow;

                _context.Documents.Add(document);
            }
            else
            {
                var existing = await _context.Documents.FirstOrDefaultAsync(d =>
                    d.Id == document.Id &&
                    d.CreatedByUserId == user.Id &&
                    d.Durum == "Draft" &&
                    !d.IsDeleted);

                if (existing == null) return NotFound();

                CopyEditableFields(existing, document);
                existing.SignerUserId = document.SignerUserId ?? "";

                _context.Documents.Update(existing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Drafts));
        }

        // =====================================================
        // SEND (POST)  => Create
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ModelDocument document)
        {
            var user = await CurrentUserAsync();
            if (user == null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(document.SignerUserId))
            {
                ModelState.AddModelError("", "İmzacı seçmelisiniz.");

                ViewBag.Users = await _context.UserProfiles
                    .Select(u => new
                    {
                        IdentityUserId = u.IdentityUserId,
                        FullName = u.FullName
                    })
                    .ToListAsync();

                return View(document);
            }

            EnsureDbSafe(document);

            if (document.Id == 0)
            {
                document.CreatedByUserId = user.Id;
                document.Durum = "Sent";
                document.CreatedAt = DateTime.UtcNow;
                document.IsDeleted = false;

                document.Sayi = await GenerateSayiAsync(document.CreatedAt);

                _context.Documents.Add(document);
            }
            else
            {
                var existing = await _context.Documents.FirstOrDefaultAsync(d =>
                    d.Id == document.Id &&
                    d.CreatedByUserId == user.Id &&
                    d.Durum == "Draft" &&
                    !d.IsDeleted);

                if (existing == null) return NotFound();

                CopyEditableFields(existing, document);
                existing.SignerUserId = document.SignerUserId ?? "";
                existing.Durum = "Sent";

                if (string.IsNullOrWhiteSpace(existing.Sayi))
                    existing.Sayi = await GenerateSayiAsync(existing.CreatedAt);

                _context.Documents.Update(existing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Outbox));
        }

        // =====================================================
        // LISTS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Drafts()
        {
            var u = await CurrentUserAsync();
            if (u == null) return Unauthorized();

            var list = await _context.Documents
                .Where(d => d.CreatedByUserId == u.Id && d.Durum == "Draft" && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Inbox()
        {
            var u = await CurrentUserAsync();
            if (u == null) return Unauthorized();

            var list = await _context.Documents
                .Where(d => d.SignerUserId == u.Id && d.Durum == "Sent" && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Outbox()
        {
            var u = await CurrentUserAsync();
            if (u == null) return Unauthorized();

            var list = await _context.Documents
                .Where(d => d.CreatedByUserId == u.Id && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        // =====================================================
        // SIGN PAGE (GET)
        // /Document/Sign/5
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Sign(int id)
        {
            var u = await CurrentUserAsync();
            if (u == null) return Unauthorized();

            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            if (doc == null) return NotFound();

            if (!string.Equals(doc.SignerUserId, u.Id, StringComparison.Ordinal))
                return Forbid();

            return View("sign", doc);
        }

        // =====================================================
        // APPROVE (POST) + ✅ EMAIL
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int documentId)
        {
            var u = await CurrentUserAsync();
            if (u == null) return Unauthorized();

            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);
            if (doc == null) return NotFound();

            if (!string.Equals(doc.SignerUserId, u.Id, StringComparison.Ordinal))
                return Forbid();

            if (doc.Durum != "Sent")
                return RedirectToAction(nameof(Sign), new { id = doc.Id });

            doc.Durum = "Approved";
            doc.ApprovedAt = DateTime.UtcNow;
            doc.RejectedAt = null;

            await _context.SaveChangesAsync();

            await NotifySenderAsync(doc,
                subject: "Belgeniz Onaylandı",
                body:
$@"Belgeniz onaylandı.

Sayı: {doc.Sayi}
Başlık: {doc.Baslik}
Konu: {doc.Konu}

EBYS");

            return RedirectToAction(nameof(Sign), new { id = doc.Id });
        }

        // =====================================================
        // REJECT (POST) + ✅ EMAIL
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int documentId)
        {
            var u = await CurrentUserAsync();
            if (u == null) return Unauthorized();

            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);
            if (doc == null) return NotFound();

            if (!string.Equals(doc.SignerUserId, u.Id, StringComparison.Ordinal))
                return Forbid();

            if (doc.Durum != "Sent")
                return RedirectToAction(nameof(Sign), new { id = doc.Id });

            doc.Durum = "Rejected";
            doc.RejectedAt = DateTime.UtcNow;
            doc.ApprovedAt = null;

            await _context.SaveChangesAsync();

            await NotifySenderAsync(doc,
                subject: "Belgeniz Reddedildi",
                body:
$@"Belgeniz reddedildi.

Sayı: {doc.Sayi}
Başlık: {doc.Baslik}
Konu: {doc.Konu}

EBYS");

            return RedirectToAction(nameof(Sign), new { id = doc.Id });
        }

        // =====================================================
        // PDF
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Pdf(int id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return NotFound();

            var pdf = PdfDocument.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text("ELEKTRONİK BELGE").Bold().FontSize(16);
                        col.Item().Text($"Sayı: {doc.Sayi}");
                        col.Item().Text($"Başlık: {doc.Baslik}");
                        col.Item().Text($"Konu: {doc.Konu}");
                        col.Item().LineHorizontal(1);
                        col.Item().Text(doc.EditorContent ?? "");
                    });
                });
            });

            return File(pdf.GeneratePdf(), "application/pdf", $"{doc.Sayi}.pdf");
        }

        // =====================================================
        // HELPERS
        // =====================================================
        private static void EnsureDbSafe(ModelDocument d)
        {
            d.Baslik ??= "(Başlıksız)";
            d.Konu ??= "";
            d.Aciklama ??= "";
            d.EditorContent ??= "";
            d.GizlilikDerecesi ??= "";
            d.UreticiBelgesi ??= "Birim";
            d.UretimYeri ??= "—";
            d.SignerUserId ??= "";
            d.Sayi ??= "";
            d.Durum ??= "Draft";
        }

        private static void CopyEditableFields(ModelDocument target, ModelDocument src)
        {
            target.BelgeTuru = src.BelgeTuru;
            target.BelgeTarihi = src.BelgeTarihi;
            target.BilaTarih = src.BilaTarih;

            target.BelgeKategorisi = src.BelgeKategorisi;
            target.Konu = src.Konu;
            target.Baslik = src.Baslik;
            target.Aciklama = src.Aciklama;

            target.UreticiBelgesi = src.UreticiBelgesi;
            target.UretimYeri = src.UretimYeri;
            target.Dil = src.Dil;

            target.TelifYasasi = src.TelifYasasi;
            target.BilgiEdinme = src.BilgiEdinme;
            target.KisiselBilgi = src.KisiselBilgi;
            target.ZorunluHal = src.ZorunluHal;
            target.OlaganustuDurum = src.OlaganustuDurum;
            target.BirimHiyerarsi = src.BirimHiyerarsi;

            target.GizlilikDerecesi = src.GizlilikDerecesi;
            target.EditorContent = src.EditorContent;
        }

        private async Task<string> GenerateSayiAsync(DateTime createdAtUtc)
        {
            int year = DateTime.Now.Year;
            int count = await _context.Documents.CountAsync(d => d.CreatedAt.Year == year) + 1;
            return $"{count}/{year}";
        }

        private async Task NotifySenderAsync(ModelDocument doc, string subject, string body)
        {
            var sender = await _userManager.FindByIdAsync(doc.CreatedByUserId);
            if (sender?.Email == null) return;

            await _email.SendAsync(sender.Email, subject, body);
        }
    }
}

#nullable restore
