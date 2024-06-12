namespace app_insights_requests;

public sealed class CustomMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.Request.Path.StartsWithSegments("/CustomMiddleware204"))
        {
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        if (httpContext.Request.Path.StartsWithSegments("/CustomMiddleware500"))
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return;
        }

        if (httpContext.Request.Path.StartsWithSegments("/CustomMiddlewareThrow"))
        {
            throw new Exception("Exception thrown from /CustomMiddlewareThrow");
        }

        await next(httpContext);
    }
}

public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<CustomMiddleware>();
}