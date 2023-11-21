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
    public IActionResult Index()
    {
        if (User.Claims.Count() == 0) return View("Views/LandingPage.cshtml");
        else return File("host.html", "text/html");
    }
}
