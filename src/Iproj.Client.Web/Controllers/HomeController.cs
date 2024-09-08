using Iproj.Client.Web.Client.Helpers;
using Iproj.Client.Web.Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Iproj.Client.Web.Client.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var claimsPrincipal = accessToken!.DecodeJwtToken();

        var userRole = claimsPrincipal.GetRole();

        if (userRole != null)
        {
            if (userRole.Equals("Owner"))
            {
                return RedirectToAction("OwnerDashboard", "Owner");
            }
            else if (userRole.Equals("Worker"))
            {
                return RedirectToAction("WorkerDashboard", "Worker");
            }
            else
            {
                return View();
            }
        }
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Logout action
    public IActionResult Logout()
    {
        Response.Cookies.Delete("Cookies");

        return SignOut(new AuthenticationProperties
        {
            RedirectUri = Url.Action("Index", "Home")
        }, "Cookies", "oidc");
    }
}
