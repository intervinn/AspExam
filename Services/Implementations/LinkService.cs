using AspExam.Data;
using AspExam.Data.Entities;
using AspExam.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspExam.Services.Implementations;

public class LinkService : ILinkService
{
    private AppDbContext _dbContext;

    public LinkService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Link?> MatchAsync(string endpoint) {
        var endpointExists = await _dbContext.Links.Where(e => e.CustomEndpoint == endpoint).FirstOrDefaultAsync();
        if (endpointExists != null)
        {
            return endpointExists;
        }

        var idExists = await _dbContext.Links.Where(e => Base62Converter.Decode(endpoint) == e.Id).FirstOrDefaultAsync();
        if (idExists != null)
        {
            return idExists;
        }

        return null;
    }

    public async Task<Link?> FindByIdAsync(string id) {
        return await _dbContext.Links.Where(e => Base62Converter.Decode(id) == e.Id).FirstOrDefaultAsync();
    }

    public async Task<Link?> FindByEndpointAsync(string endpoint) {
        return await _dbContext.Links.Where(e => endpoint == e.CustomEndpoint).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Link link) {
        await _dbContext.Links.AddAsync(link);
        await _dbContext.SaveChangesAsync();
    }
}