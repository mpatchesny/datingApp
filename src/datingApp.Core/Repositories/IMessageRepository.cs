using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetByMatchIdAsync(int matchId);
    Task AddAsync(Message message);
    Task DeleteAsync(Message message);
}