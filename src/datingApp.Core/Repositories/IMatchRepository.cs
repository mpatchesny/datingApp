using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Specifications;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Repositories;

public interface IMatchRepository
{
    Task<bool> ExistsAsync(UserId userId1, UserId userId2);
    Task<Match> GetByIdAsync(MatchId matchId, ISpecification<Match> specification = null);
    Task<IEnumerable<Match>> GetByUserIdAsync(UserId userId, ISpecification<Match> specification = null);
    Task<Match> GetByMessageIdAsync(MessageId messageId, ISpecification<Match> specification = null);
    Task AddAsync(Match match);
    Task UpdateAsync(Match match);
    Task DeleteAsync(Match match);
}