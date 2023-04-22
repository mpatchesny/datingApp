using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresMessageRepository : IMessageRepository
{
    private readonly DatingAppDbContext _dbContext;
    public PostgresMessageRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Message>> GetByMatchIdAsync(Guid matchId)
    {
        return await _dbContext.Messages.Where(x => x.MatchId == matchId).ToListAsync();
    }

    public async Task<Message> GetByIdAsync(Guid messageId)
    {
        return await _dbContext.Messages.SingleOrDefaultAsync(x => x.Id == messageId);
    }

    public async Task AddAsync(Message message)
    {
        await _dbContext.Messages.AddAsync(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Message message)
    {
        _dbContext.Messages.Update(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(Message[] messages)
    {
        _dbContext.Messages.UpdateRange(messages);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid messageId)
    {
        var message = await _dbContext.Messages.FirstAsync(x => x.Id == messageId);
        _dbContext.Messages.Remove(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetPreviousNotDisplayedMessages(Guid messageId)
    {
        var minCreatedTime = _dbContext.Messages
                                        .Where(x => x.Id == messageId)
                                        .Select(x => x.CreatedAt)
                                        .FirstOrDefault();
        return await _dbContext.Messages.Where(x => x.CreatedAt <= minCreatedTime)
                                        .Where(x => x.IsDisplayed == false).ToListAsync();
    }
}