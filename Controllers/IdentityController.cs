using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Data;
using Views.Identity.Models;

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

    [HttpPost]
    [Route("/Login")]
    public async Task<IActionResult> LoginPost([Bind] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.ApplicationUser.Where(x => x.UserName == model.Username).First();
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
            if (result.Succeeded) return Redirect("/");
            else return Redirect("/");
        }
        else
        {
            return new BadRequestResult();
        }
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
