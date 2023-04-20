using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

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
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Swipe> GetBySwipedBySwipedWhoAsync(Guid swippedById, Guid swippedWhoId)
    {
        var swipe = await _dbContext.Swipes.SingleOrDefaultAsync(x => x.SwippedById == swippedById & x.SwippedWhoId == swippedWhoId);
        return swipe;
    }
}