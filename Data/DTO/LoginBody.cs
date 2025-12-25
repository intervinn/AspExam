using System.ComponentModel.DataAnnotations;

namespace AspExam.Data.DTO;

public class LoginBody
{
    [Required]
    [EmailAddress]
    public required string Email;

    [Required]
    public required string Password;
}