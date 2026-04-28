using EBYS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBYS.ViewComponents
{
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserProfileViewComponent(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity!.IsAuthenticated)
                return View(null);

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return View(null);

            var profile = await _context.UserProfiles
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

            return View(profile);
        }
    }
}
