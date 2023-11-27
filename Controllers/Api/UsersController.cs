using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using Data;
using Services;

namespace Controllers.Api;

[StandardApiController("Users")]
[Authorize]
public class UsersController : Controller
{
    private readonly ILogger<LandingPageController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ApplicationUserManager _userManager;

    public UsersController(ILogger<LandingPageController> logger, ApplicationDbContext context, ApplicationUserManager userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("me")]
    public IActionResult GetMe()
    {
        var user = _userManager.GetLoggedInUser(HttpContext);
        if (user == null) return NotFound();

        return Json(new
        {
            Id = user.Id,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email ?? "",
        });
    }

    [HttpPost]
    [Route("me/password")]
    public async Task<IActionResult> UpdatePassword([FromBody] (string password, string confirmPassword) passwords)
    {
        var user = _userManager.GetLoggedInUser(HttpContext);
        if (user == null) return NotFound();

        if (passwords.password != passwords.confirmPassword) return BadRequest("Passwords do not match");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, token, passwords.password);

        return Ok();
    }
}