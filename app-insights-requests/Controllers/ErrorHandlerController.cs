using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace app_insights_requests.Controllers;

public class ErrorHandlerController(ILogger<ErrorHandlerController> logger) : Controller
{
    private readonly ILogger _logger = logger;

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestTelemetry = HttpContext.Features.Get<RequestTelemetry>();

        if (HttpContext.Features.Get<IExceptionHandlerPathFeature>() is { } exceptionHandlerPathFeature)
        {
            _logger.LogInformation(
                """
                Handling Exception "{ExceptionName}" for original path "{OriginalPath}", request name "{RequestName}"
                """,
                exceptionHandlerPathFeature.Error.GetType().Name,
                exceptionHandlerPathFeature.Path,
                requestTelemetry?.Name);

            return Problem();
        }

        if (HttpContext.Features.Get<IStatusCodeReExecuteFeature>() is { } statusCodeReExecuteFeature)
        {
            _logger.LogInformation(
                """
                Handling StatusCode {StatusCode} for original path "{OriginalPath}", request name "{RequestName}"
                """,
                statusCodeReExecuteFeature.OriginalStatusCode,
                statusCodeReExecuteFeature.OriginalPath,
                requestTelemetry?.Name);

            return Problem(statusCode: statusCodeReExecuteFeature.OriginalStatusCode);
        }

        _logger.LogCritical("Neither status code or exception are available!");
        return Problem("Neither status code or exception are available!");
    }
}