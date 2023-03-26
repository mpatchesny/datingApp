using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresPhotoRepository : IPhotoRepository
{
    private readonly DatingAppDbContext _dbContext;
    public PostgresPhotoRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(Photo photo)
    {
        await _dbContext.Photos.AddAsync(photo);
    }

    public Task DeleteAsync(Photo photo)
    {
        _dbContext.Photos.Remove(photo);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Photo>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Photos.Where(x=> x.UserId == userId).ToListAsync();
    }
}