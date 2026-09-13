using DataMatrix.Web.Extensions;
using DataMatrix.Web.Models.Settings;
using DataMatrix.Web.Services;
using DataMatrix.Web.Workers;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using DataMatrix.Web.Enums;
using Microsoft.AspNetCore.Authorization;
using DataMatrix.Web.Services.Interfaces;
using System.Net;


var logger = LogManager.Setup(build =>
    build.LoadConfigurationFromFile("NLog.config"))
    .GetCurrentClassLogger();

try
{
    logger.Debug("Запуск сервиса");

    var builder = WebApplication.CreateBuilder(args);
    // Add services to the container.
    var services = builder.Services;
    services.AddControllersWithViews();

    AppSettings appSettings = builder.Configuration!.GetSection("AppSettings")!.Get<AppSettings>()!;
    services.AddSingleton(appSettings.ApiHttpClientSettings);
    services.AddSingleton(appSettings.FileSettings);
    services.AddSingleton(appSettings.CleanupWorkerSettings);

    services.AddHttps();
    services.AddScoped<IFileService, FileService>();
    services.AddScoped<ISecurityService, SecurityService>();
    services.AddHostedService<CleanupWorker>();
    services.AddNLog();

    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(option =>
        {
            option.Cookie.Name = "AuthToken";

            option.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };

            option.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });

    services.AddAuthorization(options =>
    {
        foreach (var role in Enum.GetValues<Role>())
            options.AddPolicy(role.GetRole(), builder => builder.RequireRole(role.GetRole()));
    });

    services.AddHttpContextAccessor();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();


    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Остановка сервиса из-за исключения");
    throw;
}
