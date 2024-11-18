using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.Services;

internal sealed class RevokedRefreshTokensService : IRevokedRefreshTokensService
{
    DatingAppDbContext _dbContext;
    public RevokedRefreshTokensService(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> ExistsAsync(string token)
    {
        return await _dbContext.RevokedRefreshTokens.AnyAsync(x => x.Token == token);
    }
    public async Task AddAsync(TokenDto token)
    {
        await _dbContext.AddAsync(token);
        await _dbContext.SaveChangesAsync();
    }

}