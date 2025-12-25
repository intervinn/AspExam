using System.ComponentModel.DataAnnotations;

namespace AspExam.Data.DTO;

public class LoginBody
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}