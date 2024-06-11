using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace app_insights_requests.Controllers;

public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => Json(
        new
        {
            traceId = Activity.Current?.Id,
        });
}

public class TestController : Controller
{
    [AllowAnonymous]
    public void ThrowException()
    {
        throw new Exception("Exception thrown from /Test/ThrowException action");
    }

    [Authorize(Roles = "NonexistingRole")]
    public void ReturnUnauthorized()
    {
        throw new UnreachableException();
    }
}