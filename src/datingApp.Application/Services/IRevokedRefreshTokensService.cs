using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;

namespace datingApp.Application.Repositories;

public interface IRevokedRefreshTokensService
{
    public Task<bool> ExistsAsync(string token);
    public Task AddAsync(TokenDto token);
}