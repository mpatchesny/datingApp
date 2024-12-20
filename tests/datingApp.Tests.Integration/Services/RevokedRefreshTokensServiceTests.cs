using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Services;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Services;

public class RevokedRefreshTokensServiceTests : IDisposable
{
    [Fact]
    public async Task given_token_exists_ExistsAsync_returns_true()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        await _dbContext.RevokedRefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var exists = await _service.ExistsAsync(token.Token);

        Assert.True(exists);
    }

    [Fact]
    public async Task given_token_not_exists_ExistsAsync_returns_false()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        var exists = await _service.ExistsAsync(token.Token);

        Assert.False(exists);
    }

    [Fact]
    public async Task AddAsync_adds_token_to_repository()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        await _service.AddAsync(token);
        _dbContext.ChangeTracker.Clear();

        var exists = await _dbContext.RevokedRefreshTokens.AnyAsync(x => x.Token == token.Token);

        Assert.True(exists);
    }

    [Fact]
    public async Task given_token_exists_add_token_with_same_token_throws_exceptions()
    {
        var tokenToken = "abcdef";
        var token = new TokenDto(tokenToken, DateTime.UtcNow);
        await _dbContext.RevokedRefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var badToken = new TokenDto(tokenToken, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _service.AddAsync(badToken));

        Assert.NotNull(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly IRevokedRefreshTokensService _service;
    public RevokedRefreshTokensServiceTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _service = new RevokedRefreshTokensService(_dbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}