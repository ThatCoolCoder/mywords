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

    [HttpGet]
    [Route("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int amount = 2)
    {
        if (amount > 10) return BadRequest();

        // todo: orderby when we have created dates on these

        var user = _context.GetLoggedInUser(HttpContext);
        return Json(await _context.Collection
            .Where(x => x.ApplicationUserId == user.Id)
            .Take(amount)
            .Select(x => new CollectionApiModel(x.Id, x.Name, x.Description))
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateNew([FromBody] CollectionApiModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Model state invalid");

        var user = _context.GetLoggedInUser(HttpContext);

        var collection = new Collection()
        {
            ApplicationUser = user,
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
        };
        _context.Collection.Add(collection);
        await _context.SaveChangesAsync();

        return Json(new CollectionApiModel(collection.Id, collection.Name, collection.Description));
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
    public IActionResult GetTermsById(long id, [FromQuery] int? amount)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = _context.Collection
            .Include(x => x.Terms)
                .ThenInclude(x => x.LabelTerms)
                    .ThenInclude(x => x.Label)
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefault();

        if (collection == null) return NotFound();


        return Json(amount == null
            ? collection.Terms.Select(TermApiModel.FromTerm)
            : collection.Terms.OrderByDescending(x => x.CreatedUtc).Take(amount ?? 0).Select(TermApiModel.FromTerm)); // linq isn't realising we have a null-guard
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
