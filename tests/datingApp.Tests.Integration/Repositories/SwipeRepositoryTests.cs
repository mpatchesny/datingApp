using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;

[Collection("Integration tests")]
public class SwipeRepositoryTests : IDisposable
{
    [Fact]
    public async Task add_swipe_should_succeed()
    {
        var swipe = new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(swipe));
        Assert.Null(exception);
    }

    // Arrange
    private readonly ISwipeRepository _repository;
    private readonly TestDatabase _testDb;
    
    public SwipeRepositoryTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);
        
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();
        _repository = new PostgresSwipeRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}