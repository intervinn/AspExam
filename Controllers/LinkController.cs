using System.Security.Claims;
using AspExam.Data.DTO;
using AspExam.Data.Entities;
using AspExam.Helpers;
using AspExam.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("link")]
public class LinkController : ControllerBase
{
    private readonly ILinkService _linkService;
    private readonly ILogger<LinkController> _logger;

    public LinkController(ILinkService linkService, ILogger<LinkController> logger)
    {
        _linkService = linkService;
        _logger = logger;
    }

    [HttpGet("go/{route}")]
    public async Task<IActionResult> RouteLink(string route)
    {
        var result = await _linkService.MatchAsync(route);
        if (result != null)
        {
            return Redirect(result.Destination);
        } else
        {
            return NotFound();
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateLink([FromBody] CreateLinkBody body)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || body.Name == null)
        {
            _logger.LogInformation("USER NOT AUTHENTICATED");
            _logger.LogInformation($"DATA {body.Destination} {body.Name}");
            var link = new Link()
            {
                Destination = body.Destination,
                CustomEndpoint = null,
                Owner = null,
                OwnerId = null
            };

            await _linkService.CreateAsync(link);

            var destination = Base62Converter.Encode(link.Id);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Data = $"/link/go/{destination}"
            });
        } else
        {
            _logger.LogInformation("USER AUTHENTICATED");
            _logger.LogInformation($"DATA {body.Destination} {body.Name}");
            var link = new Link()
            {
                Destination = body.Destination,
                OwnerId = userId,
                CustomEndpoint = body.Name
            };

            var exists = await _linkService.FindByEndpointAsync(body.Name);
            if (exists != null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Endpoint already taken"
                });
            }

            await _linkService.CreateAsync(link);
            
            return Ok(new ApiResponse<string>
            {
                Success = false,
                Data = $"/link/go/{body.Name}"
            });
        }
    }
}