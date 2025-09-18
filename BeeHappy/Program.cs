using DataAccessObjects;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;
using Repositories.Generics;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add our repos
        SetupRepos(builder);
        // Add our services
        SetupServices(builder);

        // Configure database
        builder.Services.AddScoped<MongoDBContext>();

        // Allow page to access the session directly
        builder.Services.AddHttpContextAccessor();

        // Add session
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = System.TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Configure authentication with cookies
        // Cookie Authentication setup
        _ = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // redirect if not logged in
                options.AccessDeniedPath = "/Account/AccessDenied"; // redirect if not authorized
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/Account/signin-google";
            });

        // Debug
        Console.WriteLine($"WORKING ENVIRONMENT: {builder.Environment.EnvironmentName}");
        Console.WriteLine($"SQL string: {builder.Configuration.GetConnectionString("DefaultConnection")}");

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Config notfound-forbidden
        app.UseStatusCodePagesWithReExecute("/Errors/{0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseSession();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

    private static void SetupServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITestObjectService, TestObjectService>();
        builder.Services.AddScoped<IAuditLogService, AuditLogService>();
        builder.Services.AddScoped<IBadgeService, BadgeService>();
        builder.Services.AddScoped<IEmoteService, EmoteService>();
        builder.Services.AddScoped<IEmoteSetService, EmoteSetService>();
        builder.Services.AddScoped<IPaintService, PaintService>();
        builder.Services.AddScoped<IUserService, UserService>();
    }

    private static void SetupRepos(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITestObjectRepository, TestObjectRepository>();
        builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        builder.Services.AddScoped<IBadgeRepository, BadgeRepository>();
        builder.Services.AddScoped<IEmoteRepository, EmoteRepository>();
        builder.Services.AddScoped<IEmoteSetRepository, EmoteSetRepository>();
        builder.Services.AddScoped<IPaintRepository, PaintRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
    }
}