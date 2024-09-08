using Iproj.Client.Web.Client.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Iproj.Client.Web.Client.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class OwnerController : Controller
{
    public async Task<IActionResult> OwnerDashboard()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var claimsPrincipal = accessToken!.DecodeJwtToken();
        var userRole = claimsPrincipal.GetRole();

        if (userRole != null)
        {
            if (userRole.Equals("Owner"))
                return View();
            else
                return Redirect("https://cl.iproj.uz/Worker/WorkerDashboard");
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
