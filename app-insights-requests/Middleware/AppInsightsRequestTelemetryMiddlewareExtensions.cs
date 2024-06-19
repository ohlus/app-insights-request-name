namespace app_insights_requests.Middleware;

public static class AppInsightsRequestTelemetryMiddlewareExtensions
{
    public static IApplicationBuilder UseAppInsightsRequestTelemetryMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<AppInsightsRequestTelemetryMiddleware>();
}