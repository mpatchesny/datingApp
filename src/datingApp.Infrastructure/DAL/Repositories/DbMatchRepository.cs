using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class DbMatchRepository : IMatchRepository
{
    private readonly DatingAppDbContext _dbContext;
    public DbMatchRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(UserId userId1, UserId userId2)
    {
        return await _dbContext.Matches
                    .AnyAsync(x => x.UserId1.Equals(userId1) && x.UserId2.Equals(userId2));
    }

    public async Task<Match> GetByUserIdAsync(UserId userId1, UserId userId2)
    {
        Expression<Func<Match, bool>> predicate = x => x.UserId1.Equals(userId1) && x.UserId2.Equals(userId2);
        if (userId1.Value.CompareTo(userId2.Value) >= 0)
        {
            predicate = x => x.UserId1.Equals(userId2) && x.UserId2.Equals(userId1);
        }

        return await _dbContext.Matches.FirstOrDefaultAsync(predicate);
    }

    public async Task<Match> GetByIdAsync(MatchId matchId)
    {
        return await _dbContext.Matches
            .Include(match => match.Messages)
            .FirstOrDefaultAsync(match => match.Id == matchId);
    }

    public async Task<Match> GetByMessageIdAsync(MessageId messageId)
    {
        return await _dbContext.Matches
            .Where(match => match.Messages.Any(message => message.Id.Equals(messageId)))
            .Include(match => match.Messages)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Match match)
    {
        await _dbContext.Matches.AddAsync(match);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Match match)
    {
        _dbContext.Matches.Update(match);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Match match)
    {
        _dbContext.Matches.Remove(match);
        await _dbContext.SaveChangesAsync();
    }


}