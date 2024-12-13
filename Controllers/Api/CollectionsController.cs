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

    public record CollectionApiModel(
        long Id,
        string Name,
        string Description,
        DateTime? CreatedTimeUtc,
        DateTime? ViewedTimeUtc
    )
    {
        public static CollectionApiModel FromCollection(Collection x)
        {
            return new CollectionApiModel(x.Id, x.Name, x.Description, x.CreatedTimeUtc, x.ViewedTimeUtc);
        }
    }

    [HttpGet]
    public IActionResult GetAllForUser()
    {
        var user = _context.GetLoggedInUser(HttpContext);
        _context.Entry(user!).Collection(x => x.Collections).Load(); // todo: make this null ignore not needed
        return Json(user.Collections
            .Select(CollectionApiModel.FromCollection)
            .OrderByDescending(x => x.ViewedTimeUtc));
    }

    [HttpGet]
    [Route("recent")]
    public IActionResult GetRecent([FromQuery] int amount = 2)
    {
        if (amount > 10) return BadRequest();

        var user = _context.GetLoggedInUser(HttpContext);
        return Json(_context.Collection
            .Where(x => x.ApplicationUserId == user.Id)
            .OrderByDescending(x => x.ViewedTimeUtc)
            .Take(amount)
            .Select(CollectionApiModel.FromCollection));
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
            Description = model.Description,
            CreatedTimeUtc = DateTime.UtcNow,
            ViewedTimeUtc = DateTime.UtcNow,
        };
        _context.Collection.Add(collection);
        await _context.SaveChangesAsync();

        return Json(CollectionApiModel.FromCollection(collection));
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = await _context.Collection
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefaultAsync();
        if (collection == null) return NotFound();

        return Json(CollectionApiModel.FromCollection(collection));
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateById(long id, [FromBody] CollectionApiModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Model state invalid");

        var user = _context.GetLoggedInUser(HttpContext);
        var collection = await _context.Collection
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefaultAsync();

        if (collection == null) return NotFound();

        collection.Name = model.Name;
        collection.Description = model.Description;
        collection.ViewedTimeUtc = DateTime.UtcNow;

        _context.Update(collection);
        _context.SaveChanges();

        return Ok();
    }

    [HttpPost]
    [Route("{id}/viewed")]
    public async Task<IActionResult> OnViewed(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = await _context.Collection
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefaultAsync();

        if (collection == null) return NotFound();

        collection.ViewedTimeUtc = DateTime.UtcNow;
        _context.Update(collection);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = await _context.Collection.Where(x => x.Id == id && x.ApplicationUserId == user.Id).FirstOrDefaultAsync();

        if (collection == null) return NotFound();

        _context.Remove(collection);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    [Route("{id}/terms")]
    public async Task<IActionResult> GetTermsById(long id, [FromQuery] int? amount)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = await _context.Collection
            .Include(x => x.Terms)
                .ThenInclude(x => x.LabelTerms)
                    .ThenInclude(x => x.Label)
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id)
            .FirstOrDefaultAsync();

        if (collection == null) return NotFound();


        return Json(amount == null
            ? collection.Terms.Select(TermApiModel.FromTerm)
            : collection.Terms.OrderByDescending(x => x.CreatedUtc).Take(amount ?? 0).Select(TermApiModel.FromTerm)); // linq isn't realising we have a null-guard
    }

    [HttpGet]
    [Route("{id}/labels")]
    public async Task<IActionResult> GetLabelsById(long id)
    {
        var user = _context.GetLoggedInUser(HttpContext);
        var collection = await _context.Collection
            .Include(x => x.Labels)
            .Where(x => x.Id == id && x.ApplicationUserId == user.Id)
            .FirstOrDefaultAsync();

        if (collection == null) return NotFound();

        return Json(collection.Labels.Select(x => new LabelApiModel(x.Id, x.Name, x.Color, x.CollectionId)));
    }
}
