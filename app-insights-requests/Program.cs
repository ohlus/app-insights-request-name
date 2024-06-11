using app_insights_requests;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddBearerToken("TestScheme", options => { });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("TestPolicy", policy => policy.AddAuthenticationSchemes("TestScheme").RequireAuthenticatedUser());
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry(
    new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
    {
        ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
    });

var app = builder.Build();

// both re-execute the same controller action:
app.UseExceptionHandler("/ErrorHandler/Error");
app.UseStatusCodePagesWithReExecute("/ErrorHandler/Error");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(_ => { });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseCustomMiddleware();

app.Run();