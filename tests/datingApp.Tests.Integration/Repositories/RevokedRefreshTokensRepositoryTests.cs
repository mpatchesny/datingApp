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
    public async Task given_token_exists_in_repository_ExistsAsync_should_return_true()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        await _testDb.DbContext.RevokedRefreshTokens.AddAsync(token);
        await _testDb.DbContext.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(token.Token);
        Assert.True(exists);
    }

    [Fact]
    public async Task given_token_not_exists_in_repository_ExistsAsync_should_return_false()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        var exists = await _repository.ExistsAsync(token.Token);
        Assert.False(exists);
    }

    [Fact]
    public async Task AddAsync_should_add_token_to_repository()
    {
        var token = new TokenDto("abcdef", DateTime.UtcNow);
        await _repository.AddAsync(token);
        var exists = await _testDb.DbContext.RevokedRefreshTokens.AnyAsync(x => x.Token == token.Token);
        Assert.True(exists);
    }

    // Arrange
    private readonly IRevokedRefreshTokensRepository _repository;
    private readonly TestDatabase _testDb;
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