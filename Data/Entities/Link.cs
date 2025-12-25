
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspExam.Data.Entities;

public class Link
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public required string Destination { get; set; }
    
    public string? CustomEndpoint { get; set; }

    public string? OwnerId {get; set;} = string.Empty;
    public AppUser? Owner { get; set; }
}