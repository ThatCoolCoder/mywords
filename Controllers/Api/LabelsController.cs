using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Data;

namespace Controllers.Api;

public record LabelApiModel(long Id, string Name, string Color, long CollectionId);

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

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] LabelApiModel model)
    {
        if (!ModelState.IsValid) return BadRequest();
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var collection = _context.Collection.FirstOrDefault(x => x.ApplicationUserId == loggedInUserId && x.Id == model.CollectionId);
        if (collection == null) return NotFound($"Collection with id={model.CollectionId} not found");

        var created = new Label()
        {
            Collection = collection,
            Name = model.Name,
            Color = model.Color
        };

        _context.Add(created);
        await _context.SaveChangesAsync();

        return Ok(created.Id);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var existing = _context.Label.Include(x => x.Collection).FirstOrDefault(x => x.Id == id && x.Collection.ApplicationUserId == loggedInUserId);
        if (existing == null) return NotFound();

        _context.Remove(existing);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateById([FromBody] LabelApiModel model)
    {
        // if (ModelState.IsValid) return BadRequest();
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var existing = _context.Label
            .Where(x => x.Collection.ApplicationUserId == loggedInUserId)
            .FirstOrDefault(x => x.Id == model.Id);
        if (existing == null) return NotFound();

        existing.Name = model.Name;
        existing.Color = model.Color;
        _context.Update(existing);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
