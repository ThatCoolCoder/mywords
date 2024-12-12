using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Controllers;

public class LandingPageController : Controller
{
    private readonly ILogger<LandingPageController> _logger;
    private readonly ApplicationDbContext _context;

    public LandingPageController(ILogger<LandingPageController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Route("/")]
    public IActionResult Index()
    {
        if (!User.Claims.Any()) return View("Views/LandingPage.cshtml");
        else
        {
            SetSpaHeaders(HttpContext);
            return File("/host.html", "text/html");
        }
    }

    public static async Task ServeSpaHost(HttpContext context)
    {
        SetSpaHeaders(context);
        await context.Response.SendFileAsync("wwwroot/host.html");
        await context.Response.CompleteAsync();
    }

    private static void SetSpaHeaders(HttpContext context)
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");
    }
}