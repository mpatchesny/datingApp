using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresSwipeRepository : ISwipeRepository
{
    private readonly DatingAppDbContext _dbContext;
    public PostgresSwipeRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Swipe swipe)
    {
        await _dbContext.Swipes.AddAsync(swipe);
    }
}