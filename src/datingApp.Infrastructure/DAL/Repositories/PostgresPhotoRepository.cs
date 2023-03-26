using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresPhotoRepository : IPhotoRepository
{
    private readonly object _dbContext;
    public PostgresPhotoRepository(object dbContext)
    {
        _dbContext = dbContext;
    }
    public Task AddAsync(Photo photo)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Photo photo)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Photo>> GetByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }
}