using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
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

    public async Task<Swipe> GetBySwipedBy(UserId swipedById, UserId swipedWhoId)
    {
        var swipe = await _dbContext.Swipes.SingleOrDefaultAsync(x => x.SwipedById.Equals(swipedById) & x.SwipedWhoId.Equals(swipedWhoId));
        return swipe;
    }

    public async Task<bool> SwipeExists(UserId swipedById, UserId swipedWhoId)
    {
        return await _dbContext.Swipes.AnyAsync(x => x.SwipedById.Equals(swipedById) & x.SwipedWhoId.Equals(swipedWhoId));
    }
}