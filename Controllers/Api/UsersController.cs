using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using Data;

namespace Controllers.Api;

[StandardApiController("Users")]
[Authorize]
public class UsersController : Controller
{
    private readonly ILogger<LandingPageController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(ILogger<LandingPageController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("me")]
    public IActionResult GetMe()
    {
        var user = _context.Users.Where(x => x.Email == User.Identity!.Name).FirstOrDefault();
        if (user == null) return NotFound();

        return Json(new
        {
            Id = user.Id,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email ?? "",
        });
    }
}