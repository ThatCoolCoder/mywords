using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Data;
using Services;

namespace Controllers.Api;

[StandardApiController("Users")]
[Authorize]
public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ApplicationUserManager _userManager;

    public UsersController(ILogger<UsersController> logger, ApplicationDbContext context, ApplicationUserManager userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public record UserApiModel(string Id, string GivenName, string FamilyName, string Email);

    [HttpGet]
    [Route("me")]
    public IActionResult GetMe()
    {
        var user = _userManager.GetLoggedInUser(HttpContext);
        if (user == null) return NotFound();

        return Json(new UserApiModel(user.Id, user.GivenName, user.FamilyName, user.Email ?? ""));
    }

    [HttpPut]
    [Route("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UserApiModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Model state invalid");

        var user = _userManager.GetLoggedInUser(HttpContext);
        if (user == null) return NotFound();

        // (no setting id as it would be dumb to change it)
        user.GivenName = model.GivenName;
        user.FamilyName = model.FamilyName;
        // user.Email = model.Email; // todo: for now we're not allowing to change this either

        _context.Update(user);
        await _context.SaveChangesAsync();

        return Ok();
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