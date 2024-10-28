using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Repositories;

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetByUserIdAsync(UserId userId);
    Task<Match> GetByIdAsync(MatchId matchId);
    Task<bool> ExistsAsync(UserId userId1, UserId userId2);
    Task AddAsync(Match match);
    Task UpdateAsync(Match match);
    Task DeleteAsync(Match match);
}