using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using FluentStorage.Utils.Extensions;
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

    public async Task<List<Match>> GetByUserIdAsync(UserId userId)
    {
        return await _dbContext.Matches
            .Where(match => match.Users.Any(user => user.Id.Equals(userId)))
            .ToListAsync();
    }

    public async Task<Match> GetByIdAsync(MatchId matchId, Expression<Func<Match, IEnumerable<Message>>> includeMessage = null)
    {
        if (includeMessage == null)
        {
            includeMessage = match => match.Messages.Take(0);
        }

        return await _dbContext.Matches
            .Include(includeMessage)
            .FirstOrDefaultAsync(match => match.Id == matchId);
    }

    public async Task<Match> GetByMessageIdAsync(MessageId messageId, Expression<Func<Match, IEnumerable<Message>>> includeMessage = null)
    {
        if (includeMessage == null)
        {
            includeMessage = match => match.Messages.Take(0);
        }

        return await _dbContext.Matches
            .Where(match => match.Messages.Any(message => message.Id.Equals(messageId)))
            .Include(includeMessage)
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
        match.Delete();
        match.MatchDetails.ForEach(matchDetail => matchDetail.Delete());
        await UpdateAsync(match);
    }


}