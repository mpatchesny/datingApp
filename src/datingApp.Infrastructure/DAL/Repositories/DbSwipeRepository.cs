using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class DbSwipeRepository : ISwipeRepository
{
    private readonly DatingAppDbContext _dbContext;
    public DbSwipeRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Swipe swipe)
    {
        await _dbContext.Swipes.AddAsync(swipe);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Swipe> GetBySwipedBy(Guid swipedById, Guid swipedWhoId)
    {
        var swipe = await _dbContext.Swipes.SingleOrDefaultAsync(x => x.SwipedById == swipedById & x.SwipedWhoId == swipedWhoId);
        return swipe;
    }

    public async Task<bool> SwipeExists(Guid swipedById, Guid swipedWhoId)
    {
        return await _dbContext.Swipes.AnyAsync(x => x.SwipedById == swipedById & x.SwipedWhoId == swipedWhoId);
    }
}