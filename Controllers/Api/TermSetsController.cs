using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Data;

namespace Controllers.Api;

[StandardApiController("TermSets")]
[Authorize]
public class TermSetsController : Controller
{
    private readonly ILogger<TermsController> _logger;
    private readonly ApplicationDbContext _context;

    public TermSetsController(ILogger<TermsController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public record TermSetApiModel(long Id, string Name, string Description);

    [HttpGet]
    public IActionResult GetIndex()
    {
        var user = _context.GetLoggedInUser(HttpContext);
        _context.Entry(user!).Collection(x => x.TermSets).Load(); // todo: make this null ignore not needed
        return Json(user.TermSets.Select(x => new TermSetApiModel(x.Id, x.Name, x.Description)));
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetById(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var termSet = _context.TermSet.Where(x => x.Id == id).FirstOrDefault();
        if (termSet == null || termSet?.ApplicationUserId != user!.Id) return NotFound();

        return Json(new TermSetApiModel(termSet!.Id, termSet.Name, termSet.Description));
    }

    [HttpGet]
    [Route("{id}/terms")]
    public IActionResult GetTermsById(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var termSet = _context.TermSet
            .Include(x => x.Terms)
                .ThenInclude(x => x.LabelTerms)
                    .ThenInclude(x => x.Label)
            .Where(x => x.Id == id).FirstOrDefault();

        if (termSet == null || termSet?.ApplicationUserId != user!.Id) return NotFound();

        return Json(termSet.Terms.Select(x => new TermApiModel(
            x.Id, x.TermSetId,
            x.Value, x.Definition, x.Notes,
            x.CurrentStreak, x.MovedToCurrentListUtc,
            x.LabelTerms.Select(x => x.Label.Id).ToList())));
    }

    [HttpGet]
    [Route("{id}/labels")]
    public IActionResult GetLabelsById(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var termSet = _context.TermSet
            .Include(x => x.Labels)
            .Where(x => x.Id == id).FirstOrDefault();

        if (termSet == null || termSet?.ApplicationUserId != user!.Id) return NotFound();

        return Json(termSet.Labels.Select(x => new LabelApiModel(x.Id, x.Name, x.Color, x.TermSetId)));
    }

    [HttpPost]
    public async Task<IActionResult> PostIndex([FromBody] TermSetApiModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Model state invalid");

        var user = _context.GetLoggedInUser(HttpContext);

        _context.TermSet.Add(new()
        {
            ApplicationUser = user,
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
        });
        await _context.SaveChangesAsync();


        return Ok();
    }
}
