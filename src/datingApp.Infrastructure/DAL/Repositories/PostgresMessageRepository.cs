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

    public Task DeleteAsync(Message message)
    {
        _dbContext.Messages.Remove(message);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Message>> GetByMatchIdAsync(int matchId)
    {
        return await _dbContext.Messages.Where(x => x.MatchId == matchId).ToListAsync();
    }
}