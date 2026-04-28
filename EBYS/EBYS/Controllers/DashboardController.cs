using EBYS.Data;
using EBYS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBYS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Dashboard page
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var model = new DashboardViewModel
            {
                InboxCount = await _context.Documents.CountAsync(d =>
                    d.SignerUserId == user.Id && d.Durum == "Sent" && !d.IsDeleted),

                OutboxCount = await _context.Documents.CountAsync(d =>
                    d.CreatedByUserId == user.Id && d.Durum != "Draft" && !d.IsDeleted),

                ArchiveCount = await _context.Documents.CountAsync(d =>
                    d.IsDeleted && (d.CreatedByUserId == user.Id || d.SignerUserId == user.Id))
            };

            return View(model);
        }

        // ✅ LIVE badge counters for sidebar
        [HttpGet]
        public async Task<IActionResult> Counts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var inbox = await _context.Documents.CountAsync(d =>
                d.SignerUserId == user.Id && d.Durum == "Sent" && !d.IsDeleted);

            var outbox = await _context.Documents.CountAsync(d =>
                d.CreatedByUserId == user.Id && d.Durum != "Draft" && !d.IsDeleted);

            var archive = await _context.Documents.CountAsync(d =>
                d.IsDeleted && (d.CreatedByUserId == user.Id || d.SignerUserId == user.Id));

            return Json(new { inbox, outbox, archive });
        }
    }
}
