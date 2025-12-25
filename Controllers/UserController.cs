using System.Security.Claims;
using AspExam.Data;
using AspExam.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspExam.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private UserManager<AppUser> _userManager;
    private AppDbContext _dbContext;
    private ILogger<UserController> _logger;

    public UserController(
        UserManager<AppUser> userManager,
        AppDbContext dbContext,
        ILogger<UserController> logger
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var links = _dbContext.Links.Where(l => l.OwnerId == user.Id).ToList();

        return Ok(new
        {
           success = true,
           data = new
           {
               user.Id,
               user.Email,
               user.FirstName,
               user.LastName,
               user.CreatedAt,
               Links = links
           }
        });
    }
}