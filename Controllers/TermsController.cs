using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Data;

namespace Controllers;

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

    [HttpGet]
    public IActionResult Index()
    {
        return new JsonResult(_context.Term.ToList());
    }
}
