using AspExam.Data.Entities;

namespace AspExam.Services;

public interface ILinkService
{
    Task<Link?> MatchAsync(string endpoint);
    Task<Link?> FindByIdAsync(string id);
    Task<Link?> FindByEndpointAsync(string endpoint);
    Task CreateAsync(Link link);
}