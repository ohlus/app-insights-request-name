using app_insights_requests.Telemetry;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DataContracts;

namespace app_insights_requests.Middleware;

public sealed record RequestTelemetryNameFeature(string? RequestName);

public sealed class AppInsightsRequestTelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public AppInsightsRequestTelemetryMiddleware(
        RequestDelegate next,
        ILogger<AppInsightsRequestTelemetryMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public Task InvokeAsync(HttpContext httpContext)
    {
        // detect re-execution
        if (httpContext.Features.Get<RequestTelemetryNameFeature>() is not null)
        {
            _logger.LogInformation(
                """
                Detected middleware pipeline re-execution, ignoring new request "{Url}"...
                """,
                httpContext.Request.GetUri());

            return _next(httpContext);
        }

        var nameCandidate = AppInsightsRequestTelemetryHelpers.GetNameFromRouteContext(httpContext.Request.RouteValues)
                         ?? httpContext.Request.Path.Value;

        if (string.IsNullOrEmpty(nameCandidate))
        {
            _logger.LogInformation(
                """
                No request name detected for url "{Url}"
                """,
                httpContext.Request.GetUri());

            httpContext.Features.Set(new RequestTelemetryNameFeature(null));

            return RunNextAndSetNameAsync(httpContext);
        }

        var name = httpContext.Request.Method + " " + nameCandidate;

        _logger.LogInformation(
            """
            Request name "{RequestName}" generated for url "{Url}"
            """,
            name,
            httpContext.Request.GetUri());

        httpContext.Features.Set(new RequestTelemetryNameFeature(name));

        return RunNextAndSetNameAsync(httpContext);
    }

    private async Task RunNextAndSetNameAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        finally
        {
            SetRequestTelemetryName(httpContext);
        }
    }

    private void SetRequestTelemetryName(HttpContext httpContext)
    {
        if (httpContext.Features.Get<RequestTelemetryNameFeature>() is { RequestName: string name }
         && httpContext.Features.Get<RequestTelemetry>() is { } requestTelemetry)
        {
            _logger.LogInformation(
                """
                Setting RequestTelemetry.Name = "{RequestName}" for url "{Url}". Previous Name was "{PrevRequestName}".
                """,
                name,
                httpContext.Request.GetUri(),
                requestTelemetry.Name);

            requestTelemetry.Name = name;
        }
    }
}