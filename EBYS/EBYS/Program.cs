using EBYS.Data;
using EBYS.Models;
using EBYS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// =====================
// DATABASE
// =====================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// =====================
// IDENTITY (WITH ROLES)
// =====================
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// =====================
// DUMMY EMAIL (REQUIRED BY IDENTITY UI)
// =====================
// (Make sure the class name matches your file. If your class is EmailSender, change this.)
builder.Services.AddTransient<IEmailSender, DummyEmailSender>();

// =====================
// DOCUMENT EMAIL SERVICE (your real SMTP mailer)
// =====================
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<EmailService>();

// =====================
// MVC + RAZOR PAGES
// =====================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// =====================
// BUILD
// =====================
var app = builder.Build();

// =====================
// QUESTPDF LICENSE
// =====================
QuestPDF.Settings.License = LicenseType.Community;

// =====================
// PIPELINE
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// =====================
// ROUTING (controllers first is safest)
// =====================

// ✅ Supports attribute routing if you ever add [Route(...)]
app.MapControllers();

// ✅ Conventional MVC routes: /Document/Create etc.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ✅ Identity / Razor pages
app.MapRazorPages();

// =====================
// SEED
// =====================
await SeedData.SeedAsync(app.Services);

app.Run();
