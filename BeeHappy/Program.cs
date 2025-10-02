using CommonObjects.Configs;
using DataAccessObjects;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Net.payOS;
using PostHog;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.HelperServices;
using Services.Implementations;
using Services.Interfaces;

namespace BeeHappy;

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
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = System.TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true; 
            options.Cookie.IsEssential = true;
        });

        // Configure authentication with cookies
        // Cookie Authentication setup
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Home/LandingPage"; // redirect if not logged in
                options.AccessDeniedPath = "/Auth/AccessDenied"; // redirect if not authorized
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

        // Cookie policy tweak
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.Lax; // hoặc None nếu HTTPS
            options.Cookie.SecurePolicy = CookieSecurePolicy.None; // HTTP deploy
        });

        builder.AddPostHog();


        // Debug
        Console.WriteLine($"WORKING ENVIRONMENT: {builder.Environment.EnvironmentName}");
        Console.WriteLine($"SQL string: {builder.Configuration.GetConnectionString("MongoDB")}");

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

        // app.UseHttpsRedirection();
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
        // User related services
        builder.Services.AddScoped<IUserService, UserService>();
        // Emote related services
        builder.Services.AddScoped<IEmoteService, EmoteService>();
        builder.Services.AddScoped<IEmoteSetService, EmoteSetService>();
        // Cosmetic related services
        builder.Services.AddScoped<IBadgeService, BadgeService>();
        builder.Services.AddScoped<IPaintService, PaintService>();
        builder.Services.AddScoped<ICosmeticsService, CosmeticsService>();
        // Payment & Store
        builder.Services.AddScoped<IStoreService, StoreService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();

        // Auto Mapper Configurations
        builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

        // PayOS
        SetupPayOs(builder);
    }

    private static void SetupPayOs(WebApplicationBuilder builder)
    {
        PayOS payOs = new PayOS(
            builder.Configuration["PayOS:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment"),
            builder.Configuration["PayOS:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment"),
            builder.Configuration["PayOS:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment"));
        builder.Services.AddSingleton(payOs);
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
        builder.Services.AddScoped<IPurchaseHistoryRepository, PurchaseHistoryRepository>();
        builder.Services.AddScoped<IPremiumPlanRepository, PremiumPlanRepository>();
    }
}