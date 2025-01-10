using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Data;

namespace Controllers.Api;

public record FriendshipApiModel();
public record FriendshipLinkApiModel(Guid Id, string SenderId, bool SingleUse);

[StandardApiController("Friendships")]
public class FriendshipController : Controller
{
    private readonly ILogger<LabelsController> _logger;
    private readonly ApplicationDbContext _context;

    public FriendshipController(ILogger<LabelsController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    [Route("new/preparelink")]
    public IActionResult PrepareLink()
    {
        // Essentially create a barebones link model for it to use

        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        if (loggedInUserId == null) return StatusCode(403, "Not authenticated");

        return Json(new FriendshipLinkApiModel(Guid.NewGuid(), loggedInUserId, false));
    }
}
