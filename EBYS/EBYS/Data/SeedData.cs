using EBYS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EBYS.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // =====================
            // ROLES
            // =====================
            await EnsureRole(roleManager, "Admin");
            await EnsureRole(roleManager, "Personel");

            // =====================
            // DEPARTMENTS
            // =====================
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new Department { Name = "Rektörlük", Type = "Üst Yönetim" },
                    new Department { Name = "Mühendislik ve Mimarlık Fakültesi", Type = "Fakülte" },
                    new Department { Name = "İlahiyat Fakültesi", Type = "Fakülte" }
                );
                await context.SaveChangesAsync();
            }

            var rektorluk = await context.Departments.FirstAsync(x => x.Name == "Rektörlük");
            var mmf = await context.Departments.FirstAsync(x => x.Name == "Mühendislik ve Mimarlık Fakültesi");
            var ifak = await context.Departments.FirstAsync(x => x.Name == "İlahiyat Fakültesi");

            // =====================
            // USERS (EXPLICIT PASSWORDS)
            // =====================
            await SeedUser(userManager, context,
                email: "rektor_kastamonu@outlook.com",
                password: "Rektor@2025",
                fullName: "Ahmet Hamdi Topal",
                title: "Rektör",
                departmentId: rektorluk.Id,
                role: "Personel");

            await SeedUser(userManager, context,
                email: "rektoryardimcisi_kastamonu@outlook.com",
                password: "Admin@2025",
                fullName: "Mehmet Atalan",
                title: "Rektör Yardımcısı",
                departmentId: rektorluk.Id,
                role: "Admin");

            await SeedUser(userManager, context,
                email: "mmfdekani_kastamonu@outlook.com",
                password: "Dekan@2025",
                fullName: "İzzet Şener",
                title: "Dekan",
                departmentId: mmf.Id,
                role: "Personel");

            await SeedUser(userManager, context,
                email: "mmfogretimuyesi_kastamonu@outlook.com",
                password: "Ogretim@2025",
                fullName: "Ahmet Nusret Özalp",
                title: "Öğretim Üyesi",
                departmentId: mmf.Id,
                role: "Personel");

            await SeedUser(userManager, context,
                email: "ifdekani_kastamonu@outlook.com",
                password: "IfDekan@2025",
                fullName: "Mehmet Atalan",
                title: "Dekan",
                departmentId: ifak.Id,
                role: "Personel");

            await SeedUser(userManager, context,
                email: "ifogretimuyesi_kastamonu@outlook.com",
                password: "IfOgretim@2025",
                fullName: "Amr Mukhtar",
                title: "Öğretim Üyesi",
                departmentId: ifak.Id,
                role: "Personel");
        }

        // =====================
        // HELPERS
        // =====================

        private static async Task EnsureRole(RoleManager<IdentityRole> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedUser(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            string email,
            string password,
            string fullName,
            string title,
            int departmentId,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new Exception($"User create failed: {email}");
                }
            }
            else
            {
                // FORCE password reset
                if (await userManager.HasPasswordAsync(user))
                {
                    await userManager.RemovePasswordAsync(user);
                }

                var passResult = await userManager.AddPasswordAsync(user, password);
                if (!passResult.Succeeded)
                {
                    throw new Exception($"Password reset failed: {email}");
                }
            }

            // ROLE
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            // PROFILE
            if (!context.UserProfiles.Any(p => p.IdentityUserId == user.Id))
            {
                context.UserProfiles.Add(new UserProfile
                {
                    IdentityUserId = user.Id,
                    FullName = fullName,
                    Title = title,
                    DepartmentId = departmentId
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
