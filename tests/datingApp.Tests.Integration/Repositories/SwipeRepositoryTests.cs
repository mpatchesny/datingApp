using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;

public class SwipeRepositoryTests : IDisposable
{
    [Fact]
    public async Task add_swipe_should_succeed()
    {
        var swipe = new Swipe(0, 1, 1, Like.Like, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(swipe));
        _testDb.DbContext.SaveChanges();
        Assert.Null(exception);
    }

    // Arrange
    private readonly ISwipeRepository _repository;
    private readonly TestDatabase _testDb;
    
    public SwipeRepositoryTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var swipe = new Swipe(0, 1, 1, Like.Like, DateTime.UtcNow);
        _testDb.DbContext.Swipes.Add(swipe);
        _testDb.DbContext.SaveChanges();

        _repository = new PostgresSwipeRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}