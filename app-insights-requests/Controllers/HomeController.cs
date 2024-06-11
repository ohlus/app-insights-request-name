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

    [AllowAnonymous]
    public void Exception()
    {
        throw new InvalidOperationException("Testing exception");
    }

    [Authorize(Roles = "NonexistingRole")]
    public void Auth()
    {
        throw new InvalidOperationException("Unreachable!");
    }
}