using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetByMatchIdAsync(MatchId matchId);
    Task<IEnumerable<Message>> GetPreviousNotDisplayedMessages(MessageId messageId);
    Task<Message> GetByIdAsync(MessageId messageId);
    Task AddAsync(Message message);
    Task UpdateAsync(Message message);
    Task UpdateRangeAsync(Message[] messages);
    Task DeleteAsync(MessageId messageId);
}