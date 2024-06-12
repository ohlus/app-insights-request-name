using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace app_insights_requests.Controllers;

public class TestController : Controller
{
    [AllowAnonymous]
    public void ThrowException()
    {
        throw new Exception("Exception thrown from /Test/ThrowException action");
    }

    [Authorize("TestPolicy")]
    public void AuthorizationPolicy()
    {
        throw new UnreachableException();
    }

    [AllowAnonymous]
    public IActionResult Return500() => StatusCode(StatusCodes.Status500InternalServerError);

    [AllowAnonymous]
    public IActionResult Return401() => StatusCode(StatusCodes.Status401Unauthorized);

    [AllowAnonymous]
    [HttpPost]
    public IActionResult OnlyPostSupported() => Ok();
}