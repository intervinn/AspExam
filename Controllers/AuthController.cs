using System.Security.Claims;
using AspExam.Data.DTO;
using AspExam.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class ChangePasswordBody
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public class AssignRoleBody
{
    public required string Email { get; set; }
    public required string RoleName { get; set; }
}

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private IAuthService _authService;
    private ILogger<AuthController> _logger;
    
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterBody body)
    {
        try
        {
            var result = await _authService.RegisterAsync(body);
            return Ok(new
            {
               success = true,
               data = result 
            });
        } 
        catch (Exception e)
        {
            _logger.LogError(e, "Registration failed");
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginBody body)
    {
        try
        {
            var result = await _authService.LoginAsync(body);
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Authentication failed");
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordBody body)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await _authService.ChangePasswordAsync(userId, body.CurrentPassword, body.NewPassword);
            return Ok(new
            {
                success = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleBody body)
    {
        try
        {
            var result = await _authService.AssignRoleAsync(body.Email, body.RoleName);
            return Ok(new
            {
                success = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }
}