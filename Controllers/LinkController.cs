
using AspExam.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("link")]
public class LinkController : ControllerBase
{
    private readonly LinkService _linkService;

    public LinkController(LinkService linkService)
    {
        _linkService = linkService;
    }

    [HttpGet("{route}")]
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


}