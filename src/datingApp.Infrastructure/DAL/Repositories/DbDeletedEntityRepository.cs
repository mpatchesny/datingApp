using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class DbDeletedEntityRepository : IDeletedEntityRepository
{
    private readonly DatingAppDbContext _dbContext;
    public DbDeletedEntityRepository(DatingAppDbContext dbContext)
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

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbContext.DeletedEntities.AnyAsync(x => x.Id == id);
    }
}