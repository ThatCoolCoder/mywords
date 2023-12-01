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


    [HttpGet]
    public IActionResult Index()
    {
        return new JsonResult(_context.Term.ToList());
    }
}
