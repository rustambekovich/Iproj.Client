using Iproj.Client.Web.Client.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Iproj.Client.Web.Client.Controllers;

[Authorize]
public class WorkerController : Controller
{
    public async Task<IActionResult> WorkerDashboard()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var claimsPrincipal = accessToken!.DecodeJwtToken();
        var userRole = claimsPrincipal.GetRole();

        if (userRole != null)
        {
            if (userRole.Equals("Worker"))
                return View();
            else
                return Redirect("https://localhost:7250/Owner/OwnerDashboard");
        }

        return View();
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("Cookies");
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = Url.Action("Index", "Home")
        }, "Cookies", "oidc");
    }
}
