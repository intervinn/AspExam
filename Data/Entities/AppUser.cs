using Microsoft.AspNetCore.Identity;

namespace AspExam.Data.Entities;

public class AppUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public IList<Link> Links { get; set; } = [];

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}