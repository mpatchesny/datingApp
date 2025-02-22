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

    public async Task DeleteUserSwipes(UserId swipedById)
    {
        var where = _dbContext.Swipes.Where(s => s.SwipedById.Equals(swipedById));
        _dbContext.Swipes.RemoveRange(where);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Swipe>> GetBySwipedBySwipedWho(UserId swipedById, UserId swipedWhoId)
    {
        var swipes = await _dbContext
            .Swipes
            .Where(
                    s => s.SwipedById.Equals(swipedById) && s.SwipedWhoId.Equals(swipedWhoId) ||
                    s.SwipedById.Equals(swipedWhoId) && s.SwipedWhoId.Equals(swipedById)
                   )
            .AsNoTracking()
            .ToListAsync();
        return swipes;
    }
}