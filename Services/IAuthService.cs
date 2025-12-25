using AspExam.Data.DTO;
using AspExam.Data.Entities;

namespace AspExam.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterBody body);
    Task<AuthResponse> LoginAsync(LoginBody body);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<bool> AssignRoleAsync(string email, string roleName);
}