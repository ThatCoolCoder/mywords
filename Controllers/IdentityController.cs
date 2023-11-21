using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Data;

namespace Controllers;

public class IdentityController : Controller
{
    private readonly ILogger<LandingPageController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityController(ILogger<LandingPageController> logger, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _signInManager = signInManager;
    }

    [Route("/Login")]
    public IActionResult Login()
    {
        return View();
    }


    [Route("/Signup")]
    public IActionResult Signup()
    {
        return View();
    }

    [Route("/Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}
