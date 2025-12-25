namespace AspExam.Data.DTO;

public class AuthResponse
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public IList<string> Roles { get; set; } = [];
}