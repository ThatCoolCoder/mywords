using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Data;

namespace Controllers.Api;

[StandardApiController("Collections")]
[Authorize]
public class CollectionsController : Controller
{
    private readonly ILogger<CollectionsController> _logger;
    private readonly ApplicationDbContext _context;

    public CollectionsController(ILogger<CollectionsController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public record CollectionApiModel(long Id, string Name, string Description);

    [HttpGet]
    public IActionResult GetIndex()
    {
        var user = _context.GetLoggedInUser(HttpContext);
        _context.Entry(user!).Collection(x => x.Collections).Load(); // todo: make this null ignore not needed
        return Json(user.Collections.Select(x => new CollectionApiModel(x.Id, x.Name, x.Description)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateNew([FromBody] CollectionApiModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Model state invalid");

        var user = _context.GetLoggedInUser(HttpContext);

        _context.Collection.Add(new()
        {
            ApplicationUser = user,
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
        });
        await _context.SaveChangesAsync();


        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetById(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = _context.Collection
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefault();
        if (collection == null) return NotFound();

        return Json(new CollectionApiModel(collection!.Id, collection.Name, collection.Description));
    }

    [HttpPut]
    [Route("{id}")]
    public IActionResult UpdateById(long id, [FromBody] CollectionApiModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Model state invalid");
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = _context.Collection
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefault();

        if (collection == null) return NotFound();

        collection.Name = model.Name;
        collection.Description = model.Description;

        _context.Update(collection);
        _context.SaveChanges();

        return Ok();
    }

    [HttpGet]
    [Route("{id}/terms")]
    public IActionResult GetTermsById(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = _context.Collection
            .Include(x => x.Terms)
                .ThenInclude(x => x.LabelTerms)
                    .ThenInclude(x => x.Label)
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefault();

        if (collection == null) return NotFound();
        
        return Json(collection.Terms.Select(x => new TermApiModel(
            x.Id, x.CollectionId,
            x.Value, x.Definition, x.Notes,
            (int) x.TermList, x.CurrentStreak, x.MovedToCurrentListUtc,
            x.LabelTerms.Select(x => x.Label.Id).ToList())));
    }

    [HttpGet]
    [Route("{id}/labels")]
    public IActionResult GetLabelsById(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = _context.Collection
            .Include(x => x.Labels)
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefault();

        if (collection == null) return NotFound();

        return Json(collection.Labels.Select(x => new LabelApiModel(x.Id, x.Name, x.Color, x.CollectionId)));
    }
}
