
using System.ComponentModel.DataAnnotations;

namespace AspExam.Data.DTO;

public class RegisterBody
{
    [Required]
    public required string FirstName;

    [Required]
    public required string LastName;

    [Required]
    [EmailAddress]
    public required string Email;

    [Required]
    [MinLength(8)]
    public required string Password;
}