using System.ComponentModel.DataAnnotations;

namespace AspExam.Data.DTO;
public class CreateLinkBody
{
    [Required(ErrorMessage = "Destination is required")]
    public required string Destination { get; set; }
    public string? Name { get; set; }
}
