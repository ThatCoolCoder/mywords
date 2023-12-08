using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Data;

namespace Controllers.Api;

public record LabelApiModel(long Id, string Name, string Color, long TermSetId);

[StandardApiController("Labels")]
public class LabelsController : Controller
{
    private readonly ILogger<LabelsController> _logger;
    private readonly ApplicationDbContext _context;

    public LabelsController(ILogger<LabelsController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }


    // [HttpGet]
    // public IActionResult Index()
    // {
    //     return new JsonResult(_context.Term.ToList());
    // }

    [HttpPost]
    [Route("{id}")]
    public async Task<IActionResult> Create([FromBody] LabelApiModel model)
    {
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var termSet = _context.TermSet.First(x => x.ApplicationUserId == loggedInUserId && x.Id == model.TermSetId);
        if (termSet == null) return NotFound($"Term set with id={model.TermSetId} not found");

        var created = new Label()
        {
            TermSet = termSet,
            Name = model.Name,
            Color = model.Color
        };

        _context.Add(created);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("{id}/delete")]
    public async Task<IActionResult> Delete(long id)
    {
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var existing = _context.Label.Include(x => x.TermSet).First(x => x.Id == id && x.TermSet.ApplicationUserId == loggedInUserId);
        if (existing == null) return NotFound();

        _context.Remove(existing);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateById([FromBody] LabelApiModel model)
    {
        if (ModelState.IsValid) return BadRequest();
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var existing = _context.Label
            .Where(x => x.TermSet.ApplicationUserId == loggedInUserId)
            .First(x => x.Id == model.Id);
        if (existing == null) return NotFound();

        existing.Name = model.Name;
        existing.Color = model.Color;
        _context.Update(existing);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
