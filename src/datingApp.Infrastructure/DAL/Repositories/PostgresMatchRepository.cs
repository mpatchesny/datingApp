using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresMatchRepository : IMatchRepository
{
    private readonly object _dbContext;
    public PostgresMatchRepository(object dbContext)
    {
        _dbContext = dbContext;
    }
    public Task AddAsync(Match match)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Match match)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Match>> GetByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }
}