using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace app_insights_requests.Telemetry;

public class RequestTelemetryInitializer : TelemetryInitializerBase
{
    public RequestTelemetryInitializer(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
    }

    protected override void OnInitializeTelemetry(
        HttpContext platformContext,
        RequestTelemetry requestTelemetry,
        ITelemetry telemetry)
    {
        // THIS DOESN'T WORK, IT TRIGGERS ONLY AFTER RE-EXECUTION
        var name = AppInsightsRequestTelemetryHelpers.GetNameFromRouteContext(platformContext.Request.RouteValues);
        if (!string.IsNullOrEmpty(name))
        {
            name = platformContext.Request.Method + " " + name;
            requestTelemetry.Name = name;
        }
    }
}