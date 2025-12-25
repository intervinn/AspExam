using AspExam.Data.Entities;

namespace AspExam.Data.DTO;

public class PartialUser
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public IList<Link> Links { get; set; } = [];

    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public required string Id { get; set; }
    public required string Email { get; set; }
}