using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspExam.Data.DTO;
using AspExam.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AspExam.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterBody body)
    {
        var userExists = await _userManager.FindByEmailAsync(body.Email);
        if (userExists != null) throw new Exception("User already exists");

        var user = new AppUser
        {
            Email = body.Email,
            UserName = body.Email,
            FirstName = body.FirstName,
            LastName = body.LastName,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, body.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        
        await _userManager.AddToRoleAsync(user, "User");
        return await GenerateToken(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginBody body)
    {
        var user = await _userManager.FindByEmailAsync(body.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, body.Password))
        {
            throw new Exception("Invalid credentials");
        }

        return await GenerateToken(user);
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User doesn't exist");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> AssignRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User doesn't exist");
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            throw new Exception("Role doesn't exist");
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    private async Task<AuthResponse> GenerateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, $"{user.FirstName} {user.LastName}")
        };

        var userRoles = await _userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new MissingFieldException("Secret is required")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
            signingCredentials: credentials
        );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
            Expiration = token.ValidTo,
            Email = user.Email!,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = [.. userRoles]
        };
    }
}