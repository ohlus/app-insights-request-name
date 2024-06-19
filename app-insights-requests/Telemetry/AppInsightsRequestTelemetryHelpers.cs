namespace app_insights_requests.Telemetry;

public static class AppInsightsRequestTelemetryHelpers
{
    public static string? GetNameFromRouteContext(RouteValueDictionary routeValues)
    {
        // same logic that AI dotnet uses:
        // NETCORE/src/Microsoft.ApplicationInsights.AspNetCore/DiagnosticListeners/Implementation/HostingDiagnosticListener.cs

        if (routeValues.Count <= 0)
        {
            return null;
        }

        string? name = null;
        routeValues.TryGetValue("controller", out var controller);
        var controllerString = (controller is null) ? string.Empty : controller.ToString();

        if (!string.IsNullOrEmpty(controllerString))
        {
            name = controllerString;

            if (routeValues.TryGetValue("action", out var action) && action != null)
            {
                name += "/" + action.ToString();
            }

            if (routeValues.Keys.Count > 2)
            {
                // Add parameters
                var sortedKeys = routeValues.Keys
                    .Where(
                        key => !string.Equals(key, "controller", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(key, "action", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(key, "!__route_group", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (sortedKeys.Length > 0)
                {
                    var arguments = string.Join('/', sortedKeys);
                    name += " [" + arguments + "]";
                }
            }
        }
        else
        {
            routeValues.TryGetValue("page", out var page);
            var pageString = (page is null) ? string.Empty : page.ToString();
            if (!string.IsNullOrEmpty(pageString))
            {
                name = pageString;
            }
        }

        return name;
    }
}