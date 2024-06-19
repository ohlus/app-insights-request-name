using app_insights_requests;
using app_insights_requests.Middleware;
using app_insights_requests.Telemetry;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddBearerToken("TestScheme", options => { });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("TestPolicy", policy => policy.AddAuthenticationSchemes("TestScheme").RequireAuthenticatedUser());
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry(
    new ApplicationInsightsServiceOptions
    {
        ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
    });
// doesn't work :/
// builder.Services.AddSingleton<ITelemetryInitializer, RequestTelemetryInitializer>();

var app = builder.Build();

// both re-execute the same controller action:
app.UseExceptionHandler("/ErrorHandler/Error");
app.UseStatusCodePagesWithReExecute("/ErrorHandler/Error");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
// this works
app.UseAppInsightsRequestTelemetryMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(_ => { });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseCustomMiddleware();

app.Run();