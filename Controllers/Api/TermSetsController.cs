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

    [HttpGet]
    public IActionResult Index()
    {
        return new JsonResult(_context.TermSet.ToList());
    }
}
