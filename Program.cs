using EDSU_SYSTEM.Data;
using EDSU_SYSTEM.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MySqlConnector;
using System;
using static EDSU_SYSTEM.Models.Enum;


// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
services.AddDatabaseDeveloperPageExceptionFilter();


services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;        // Remove the requirement for a digit
    options.Password.RequireLowercase = false;    // Remove the requirement for a lowercase letter
    options.Password.RequireUppercase = false;    // Remove the requirement for an uppercase letter
    options.Password.RequireNonAlphanumeric = false; // Remove the requirement for a non-alphanumeric character
    options.Password.RequiredLength = 6;         // Set your desired minimum password length

    // Other configurations for Default Identity (if needed)
    //options.SignIn.RequireConfirmedAccount = true; // Set to true if you require account confirmation via email
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();





services.AddControllersWithViews()
        .AddNewtonsoftJson();
services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
string baseDomainUrl = "https://edouniversity.edu.ng";
services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
            options.CallbackPath = "/signin-google";
        });

var app = builder.Build();



//Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//app.UseDeveloperExceptionPage();
//app.UseMigrationsEndPoint();
//}
//else
//{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
   
 //}



    app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
