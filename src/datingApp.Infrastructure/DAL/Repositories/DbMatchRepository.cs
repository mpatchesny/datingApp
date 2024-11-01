using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.Specifications;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<Match>> GetByUserIdAsync(UserId userId, ISpecification<Match> specification)
    {
        var query = GetBaseGetQuery(specification);
        query = query.Where(match => match.UserId1.Equals(userId) || match.UserId2.Equals(userId));
        return await query.ToListAsync();
    }

    public async Task<Match> GetByIdAsync(MatchId matchId, ISpecification<Match> specification)
    {
        var query = GetBaseGetQuery(specification);
        return await query.FirstOrDefaultAsync(match => match.Id == match.Id);
    }

    public async Task<Match> GetByMessageIdAsync(MessageId messageId, ISpecification<Match> specification)
    {
        var query = GetBaseGetQuery(specification);
        query = query.Where(match => match.Messages.Any(message => message.Id == messageId));
        return await query.FirstOrDefaultAsync();
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

    private IQueryable<Match> GetBaseGetQuery(ISpecification<Match> specification)
    {
        var query = _dbContext.Matches.AsQueryable<Match>();
        if (specification != null) query = specification.Apply(query);
        return query;
    }
}