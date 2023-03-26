using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresSwipeRepository : ISwipeRepository
{
    private readonly object _dbContext;
    public PostgresSwipeRepository(object dbContext)
    {
        _dbContext = dbContext;
    }
    public Task AddAsync(Swipe swipe)
    {
        throw new NotImplementedException();
    }
}