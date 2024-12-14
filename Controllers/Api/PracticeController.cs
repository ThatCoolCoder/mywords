using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using Data;
using Services;
using System.Runtime.CompilerServices;

namespace Controllers.Api;




[StandardApiController("practice")]
public class PracticeController : Controller
{
    private readonly ILogger<PracticeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly TermPracticeService _termPracticeService;

    public PracticeController(ILogger<PracticeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
        _termPracticeService = new TermPracticeService(_context); // we need to share the context, not sure if this is the best way to do it though
    }

    [HttpPost]
    [Route("newround/{collectionId}")]
    public IActionResult CreateRound([FromRoute] long collectionId, [FromBody] TermPracticeService.PracticeSettings settings)
    {
        var collection = _context.Collection
            .Where(c => c.Id == collectionId)
            .Include(c => c.Labels)
            .FirstOrDefault();

        if (collection == null) return NotFound();
        if (collection.ApplicationUserId != _context.GetLoggedInUser(HttpContext).Id) return Unauthorized();

        var termIds = _termPracticeService.GeneratePracticeRound(collection, settings).Select(x => x.Id);

        return Json(termIds);
    }

    [HttpPost]
    [Route("submitanswer/{termId}/{result}")]
    public async Task<IActionResult> SubmitAnswer([FromRoute] long termId, [FromRoute] string result)
    {
        bool correct;
        if (result.ToLower() == "correct") correct = true;
        else if (result.ToLower() == "incorrect") correct = false;
        else return BadRequest("Expected result to be either correct or incorrect");

        var term = _context.Term.Where(t => t.Id == termId).Include(t => t.Collection).FirstOrDefault();

        if (term == null) return NotFound("Term does not exist");
        if (term.Collection.ApplicationUserId != _context.GetLoggedInUser(HttpContext).Id) return Unauthorized();

        TermPracticeService.PracticeAnswerResult practiceResult;
        try
        {
            practiceResult = await _termPracticeService.SubmitAnswer(term, correct);
        }
        catch (TermPracticeService.PracticingBackLogTerm)
        {
            return BadRequest("Cannot practice a term in the backlog list");
        }

        return Json(new { Result = (int)practiceResult, Term = TermApiModel.FromTerm(term) });
    }

    [HttpGet]
    [Route("settings/default")]
    // I can't be bothered duplicating them into js
    public IActionResult GetDefaultSettings()
    {
        return Json(new TermPracticeService.PracticeSettings());
    }
}
