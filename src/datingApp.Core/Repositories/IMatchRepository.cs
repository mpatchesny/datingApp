using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Repositories;

public interface IMatchRepository
{
    Task<Match> GetByIdAsync(MatchId matchId);
    Task<Match> GetByMessageIdAsync(MessageId messageId);
    Task AddAsync(Match match);
    Task UpdateAsync(Match match);
    Task DeleteAsync(Match match);
}