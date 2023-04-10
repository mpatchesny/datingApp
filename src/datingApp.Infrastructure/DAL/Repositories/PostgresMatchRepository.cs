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
    public async Task<IEnumerable<Match>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.Matches.Where(x => x.UserId1 == userId || x.UserId2 == userId)
                        .Include(match => match.Messages
                                .OrderByDescending(message => message.CreatedAt)
                                .Take(1))
                        .ToListAsync();
    }
    public async Task<Match> GetByIdAsync(Guid matchId)
    {
        return await _dbContext.Matches.FirstOrDefaultAsync(x => x.Id == matchId);
    }

    public async Task AddAsync(Match match)
    {
        await _dbContext.Matches.AddAsync(match);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Match match)
    {
        _dbContext.Matches.Remove(match);
        await _dbContext.SaveChangesAsync();
    }
}