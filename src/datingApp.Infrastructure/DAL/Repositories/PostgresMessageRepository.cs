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
    
    public async Task AddAsync(Message message)
    {
        await _dbContext.Messages.AddAsync(message);
    }

    public async Task DeleteAsync(Guid messageId)
    {
        var message = await _dbContext.Messages.FirstAsync(x => x.Id == messageId);
        _dbContext.Messages.Remove(message);
    }

    public async Task<IEnumerable<Message>> GetByMatchIdAsync(Guid matchId)
    {
        return await _dbContext.Messages.Where(x => x.MatchId == matchId).ToListAsync();
    }
}