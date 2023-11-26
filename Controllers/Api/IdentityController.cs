using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

using Data;

namespace Controllers.Api;

[StandardApiController("Identity")]
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

    [HttpPost]
    public async Task<IActionResult> Login([Bind] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.ApplicationUser.Where(x => x.UserName == model.Username).First();
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
            if (result.Succeeded) return Ok();
            else return Unauthorized();
        }
        else
        {
            return new BadRequestResult();
        }
    }

    [HttpPost]
    public IActionResult Signup()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}

public class LoginModel
{
    [Required] public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}