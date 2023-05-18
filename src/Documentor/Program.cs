using Documentor;
using Documentor.Framework.Routes;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using NLog;
using NLog.Web;
using Structr.AspNetCore.Rewrite;
using WebMarkupMin.AspNetCore7;

Logger logger = LogManager
    .Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    var environment = builder.Environment;
    var services = builder.Services;

    services.AddApp(configuration, environment);

    var app = builder.Build();

    if (environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseRewriter(new RewriteOptions()
        .AddRedirectToLowercase());

    bool useHttps = configuration.GetValue<bool>("UseHttps");
    if (useHttps)
    {
        app.UseHttpsRedirection();
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = context =>
        {
            ResponseHeaders headers = context.Context.Response.GetTypedHeaders();
            if (environment.IsDevelopment())
            {
                headers.CacheControl = new CacheControlHeaderValue()
                {
                    NoCache = true,
                    NoStore = true
                };
                headers.Expires = DateTime.Now.AddDays(-1);
            }
            else
            {
                headers.CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(365)
                };
            }
        }
    });
    app.UseRouting();
    app.UseWebMarkupMin();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "error",
        pattern: "Error",
        new { controller = "Errors", action = "Error" });

    app.MapControllerRoute(
        name: "manage",
        pattern: "m/{controller}/{action}",
        new { controller = "Home", action = "Index" },
        new { isPage = new PageConstraint() });

    app.MapControllerRoute(
       name: "edit-page",
       pattern: "m/Edit/{*virtualPath}",
       new { controller = "Pages", action = "Edit" });

    app.MapControllerRoute(
       name: "page",
       pattern: "{*virtualPath}",
       new { controller = "Pages", action = "Page" });

    app.MapControllerRoute(
       name: "default",
       pattern: "",
       new { controller = "Pages", action = "Page" });

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Program stopped because of exception.");
    throw;
}
finally
{
    LogManager.Shutdown();
}