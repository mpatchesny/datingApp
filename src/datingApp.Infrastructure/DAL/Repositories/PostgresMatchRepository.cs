using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresMatchRepository : IMatchRepository
{
    private readonly DatingAppDbContext _dbContext;
    public PostgresMatchRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Match match)
    {
        await _dbContext.Matches.AddAsync(match);
    }

    public Task DeleteAsync(Match match)
    {
        _dbContext.Matches.Remove(match);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Match>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Matches.Where(x => x.UserId1 == userId || x.UserId2 == userId).ToListAsync();
    }
}