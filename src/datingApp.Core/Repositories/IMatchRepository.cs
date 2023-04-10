using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetByUserIdAsync(Guid userId);
    Task<Match> GetByIdAsync(Guid matchId);
    Task AddAsync(Match match);
    Task DeleteAsync(Match match);
}