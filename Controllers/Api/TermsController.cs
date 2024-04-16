using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Controllers.Api;

public record TermApiModel(long Id, long CollectionId, string Value, string Definition, string Notes, int TermList, int CurrentStreak = 0,
    // DateTime? MovedToCurrentListUtc = null,
    DateTime? MovedToCurrentListUtc = null, DateTime? CreatedUtc = null,
    List<long> Labels = null!)
{
    public DateTime? MovedToCurrentListUtc { get; init; } = MovedToCurrentListUtc ?? DateTime.UtcNow; // c# records a little goofy with not having a better way of a non-constant default on value type
    public DateTime? CreatedUtc { get; init; } = CreatedUtc ?? DateTime.UtcNow;
    public List<long> Labels { get; init; } = Labels ?? new();

    public static TermApiModel FromTerm(Term t)
    {
        return new TermApiModel(
            t.Id, t.CollectionId,
            t.Value, t.Definition, t.Notes,
            (int)t.TermList, t.CurrentStreak, t.MovedToCurrentListUtc, t.CreatedUtc,
            t.LabelTerms.Select(x => x.Label.Id).ToList());
    }
}

[StandardApiController("Terms")]
public class TermsController : Controller
{
    private readonly ILogger<TermsController> _logger;
    private readonly ApplicationDbContext _context;

    public TermsController(ILogger<TermsController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] TermApiModel model)
    {
        if (!ModelState.IsValid) return BadRequest();
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var collection = _context.Collection.FirstOrDefault(x => x.ApplicationUserId == loggedInUserId && x.Id == model.CollectionId);
        if (collection == null) return NotFound($"Collection with id={model.CollectionId} not found");

        var created = new Term()
        {
            Collection = collection,
            Value = model.Value,
            Definition = model.Definition,
            Notes = model.Notes,
            CurrentStreak = 0,
            TermList = (TermList)model.TermList,
            MovedToCurrentListUtc = (DateTime)model.MovedToCurrentListUtc!,
            CreatedUtc = DateTime.Now,
        };
        created.LabelTerms = model.Labels.Select(id => new LabelTerm()
        {
            LabelId = id,
            Term = created
        }).ToList();

        _context.Add(created);
        await _context.SaveChangesAsync();

        return Json(TermApiModel.FromTerm(created));
    }

    [HttpPost]
    [Route("{id}/delete")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var existing = _context.Term.FirstOrDefault(x => x.Id == id && x.Collection.ApplicationUserId == loggedInUserId);
        if (existing == null) return NotFound();

        _context.Remove(existing);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateById([FromBody] TermApiModel model)
    {
        // if (ModelState.IsValid) return BadRequest();
        var loggedInUserId = _context.GetLoggedInUser(HttpContext).Id;
        var existing = _context.Term
            .Include(x => x.LabelTerms)
            .Where(x => x.Collection.ApplicationUserId == loggedInUserId)
            .FirstOrDefault(x => x.Id == model.Id);
        if (existing == null) return NotFound();

        existing.Value = model.Value;
        existing.Definition = model.Definition;
        existing.Notes = model.Notes;
        existing.CurrentStreak = model.CurrentStreak;
        if (model.TermList != (int)existing.TermList)
        {
            existing.MovedToCurrentListUtc = DateTime.UtcNow;
            existing.CurrentStreakWithinList = 0;
        }
        existing.TermList = (TermList)model.TermList;
        _context.Update(existing);

        // Add ones that didn't exist before
        // Possible performance improvement: use hashset
        // Could also extract basic logic if we have to do a many-to-many diff somewhere else
        var oldLabelIds = existing.LabelTerms.Select(x => x.LabelId);
        foreach (var newLabelId in model.Labels)
        {
            if (!oldLabelIds.Contains(newLabelId))
            {
                _context.Update(new LabelTerm() { LabelId = newLabelId, TermId = model.Id });
            }
        }
        // Remove ones that don't exist anymore
        foreach (var oldLabelTerm in existing.LabelTerms)
        {
            if (!model.Labels.Contains(oldLabelTerm.LabelId))
            {
                _context.Remove(oldLabelTerm);
            }
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
}
