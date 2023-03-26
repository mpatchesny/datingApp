using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresMessageRepository : IMessageRepository
{
    public PostgresMessageRepository()
    {
        // todo
    }
    public Task AddAsync(Message message)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Message message)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Message>> GetByMatchIdAsync(int matchId)
    {
        throw new NotImplementedException();
    }
}