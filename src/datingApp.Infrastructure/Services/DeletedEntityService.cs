using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.Services;

internal sealed class DeletedEntityService : IDeletedEntityService
{
    private readonly DatingAppDbContext _dbContext;
    public DeletedEntityService(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Guid id)
    {
        var deletedEntity = new DeletedEntityDto {
            Id = id,
        };
        await _dbContext.AddAsync(deletedEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<Guid> ids)
    {
        var deletedEntities = ids.Select(id => new DeletedEntityDto { Id = id }).ToList();
        await _dbContext.AddRangeAsync(deletedEntities);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbContext.DeletedEntities.AnyAsync(x => x.Id == id);
    }
}