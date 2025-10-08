using Microsoft.AspNetCore.Mvc;

namespace BeeHappy.Controllers.Auth;

public class ExtensionController : Controller
{
    public IActionResult AuthBridge()
    {
        return View();
    }

    public IActionResult AuthComplete(string token)
    {
        ViewBag.Token = token;
        return View();
    }
}