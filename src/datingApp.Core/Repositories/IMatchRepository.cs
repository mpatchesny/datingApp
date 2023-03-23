using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace datingApp.Core.Repositories;

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetByUserIdAsync(int userId);
    Task AddAsync(Match match);
    Task DeleteAsync(Match match);
}