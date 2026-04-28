using EBYS.Data;
using EBYS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBYS.Controllers
{
    [Authorize]
    public class AttachmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AttachmentController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int documentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            var doc = await _context.Documents.FindAsync(documentId);
            if (doc == null) return NotFound();

            var folder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(folder);

            var storedName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(folder, storedName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            _context.DocumentAttachments.Add(new DocumentAttachment
            {
                DocumentId = documentId,
                FileName = file.FileName,
                StoredFileName = storedName,
                ContentType = file.ContentType,
                FileSize = file.Length
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Sign", "Document", new { id = documentId });
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var att = await _context.DocumentAttachments.FindAsync(id);
            if (att == null) return NotFound();

            var path = Path.Combine(_env.WebRootPath, "uploads", att.StoredFileName);
            return PhysicalFile(path, att.ContentType, att.FileName);
        }
    }
}
