// using System.Collections.Generic;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;

// using Data;

// namespace Controllers.Api;


// [StandardApiController("Labels")]
// public class PracticeController : Controller
// {
//     private readonly ILogger<PracticeController> _logger;
//     private readonly ApplicationDbContext _context;

//     public PracticeController(ILogger<PracticeController> logger, ApplicationDbContext context)
//     {
//         _logger = logger;
//         _context = context;
//     }

//     [HttpGet]
//     [Route("newround/{collectionId}")]
//     public async Task<IActionResult> CreateRound([FromRoute] long collectionId)
//     {
//         return Json()
//     }

//     [HttpPost]
//     [Route("")]
// }
