using Microsoft.AspNetCore.Mvc;
using Data;

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
    public async Task<IActionResult> Index()
    {
        if (User.Claims.Count() == 0) return View("Views/LandingPage.cshtml");
        else
        {
            await ServeSpaHost(HttpContext);
            return Ok();
        }
    }

    public static async Task ServeSpaHost(HttpContext context)
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");

        await context.Response.SendFileAsync("wwwroot/host.html");
        await context.Response.CompleteAsync();
    }
}