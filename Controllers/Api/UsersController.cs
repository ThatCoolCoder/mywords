using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Data;
using Services;
using Microsoft.EntityFrameworkCore;

namespace Controllers.Api;

[StandardApiController("Users")]
[Authorize]
public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ApplicationUserManager _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UsersController(ILogger<UsersController> logger, ApplicationDbContext context, ApplicationUserManager userManager, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
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

        if (!user.IsValid()) return BadRequest();
        user.Sanitize();

        _context.Update(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    public record StartChangePasswordModel(string password);

    [HttpPost]
    [Route("me/password/generatechangetoken")]
    public async Task<IActionResult> StartChangePassword([FromBody] StartChangePasswordModel data)
    {
        if (!ModelState.IsValid) return BadRequest("Model state invalid");

        // I want to have it as a json but I don't want to create a class and c# isn't allwoing 1 element tuples so we have optional fake

        var user = _userManager.GetLoggedInUser(HttpContext);
        if (user == null) return Unauthorized();

        Console.WriteLine(data.password);

        var result = await _signInManager.CheckPasswordSignInAsync(user, data.password, true);

        if (!result.Succeeded) return Unauthorized();

        return Json(new { Token = await _userManager.GeneratePasswordResetTokenAsync(user) });
    }

    public record ChangePasswordModel(string password, string confirmPassword, string token);

    [HttpPost]
    [Route("me/password/submitchange")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel data)
    {
        if (!ModelState.IsValid) return BadRequest("Model state invalid");

        var user = _userManager.GetLoggedInUser(HttpContext);
        if (user == null) return Unauthorized();

        if (data.password != data.confirmPassword) return BadRequest("Passwords do not match");

        var result = await _userManager.ResetPasswordAsync(user, data.token, data.password);

        if (result.Succeeded) return Ok();
        else return Unauthorized();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _context.Users
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (user == null) return NotFound();

        return Json(user);
    }
}