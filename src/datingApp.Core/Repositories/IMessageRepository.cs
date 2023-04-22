using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetByMatchIdAsync(Guid matchId);
    Task<IEnumerable<Message>> GetPreviousNotDisplayedMessages(Guid messageId);
    Task<Message> GetByIdAsync(Guid messageId);
    Task AddAsync(Message message);
    Task UpdateAsync(Message message);
    Task UpdateRangeAsync(Message[] messages);
    Task DeleteAsync(Guid messageId);
}