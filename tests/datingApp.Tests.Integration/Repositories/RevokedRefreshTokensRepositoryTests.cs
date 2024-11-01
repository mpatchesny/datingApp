using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;

public class RevokedRefreshTokensRepositoryTests : IDisposable
{
    [Fact]
    public async Task given_token_exists_ExistsAsync_returns_true()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        var dbContext = _testDb.CreateNewDbContext();
        await dbContext.RevokedRefreshTokens.AddAsync(token);
        await dbContext.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(token.Token);
        Assert.True(exists);
    }

    [Fact]
    public async Task given_token_not_exists_ExistsAsync_returns_false()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        var exists = await _repository.ExistsAsync(token.Token);
        Assert.False(exists);
    }

    [Fact]
    public async Task AddAsync_adds_token_to_repository()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        await _repository.AddAsync(token);
        var exists = await _testDb.CreateNewDbContext().RevokedRefreshTokens.AnyAsync(x => x.Token == token.Token);
        Assert.True(exists);
    }

    [Fact]
    public async Task given_token_exists_add_token_with_same_token_throws_exceptions()
    {
        var tokenToken = "abcdef";
        var token = new TokenDto(tokenToken, DateTime.UtcNow);
        var dbContext = _testDb.CreateNewDbContext();
        await dbContext.RevokedRefreshTokens.AddAsync(token);
        await dbContext.SaveChangesAsync();

        var badToken = new TokenDto(tokenToken, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(badToken));
        Assert.NotNull(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly IRevokedRefreshTokensRepository _repository;
    public RevokedRefreshTokensRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repository = new DbRevokedRefreshTokensRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}