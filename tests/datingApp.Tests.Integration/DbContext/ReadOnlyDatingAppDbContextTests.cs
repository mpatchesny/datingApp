using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.DAL;
using Xunit;

namespace datingApp.Tests.Integration.DbContext;

[Collection("DbContext tests")]
public class ReadOnlyDatingAppDbContextTests : IDisposable
{
    [Fact]
    public void SaveChanges_throws_InvalidOperationException_1()
    {
        var exception = Record.Exception(() => _dbContext.SaveChanges());

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void SaveChanges_throws_InvalidOperationException_2()
    {
        var exception = Record.Exception(() => _dbContext.SaveChanges(true));

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task SaveChangesAsync_throws_InvalidOperationException_1()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _dbContext.SaveChangesAsync(CancellationToken.None));

        Assert.Equal("Cannot save changes in read-only db context.", exception.Message);
    }

    [Fact]
    public async Task SaveChangesAsync_throws_InvalidOperationException_2()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _dbContext.SaveChangesAsync(true, CancellationToken.None));

        Assert.Equal("Cannot save changes in read-only db context.", exception.Message);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly ReadOnlyDatingAppDbContext _dbContext;

    public ReadOnlyDatingAppDbContextTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.ReadOnlyDbContext;
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}