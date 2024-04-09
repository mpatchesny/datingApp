using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class SwipeRepositoryTests : IDisposable
{
    [Fact]
    public async Task add_swipe_should_succeed()
    {
        var swipe = new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(swipe));
        Assert.Null(exception);
        var addedSwipe = await _testDb.DbContext.Swipes.FirstOrDefaultAsync(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Same(swipe, addedSwipe);
    }

    [Fact]
    public async Task add_swipe_with_existing_id_should_throw_exception()
    {
        var swipe = new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow);
        await _repository.AddAsync(swipe);
        var swipe2 = new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(swipe2));
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task get_by_swiped_by_swiped_who_should_return_nonempty_collection()
    {
        var swipes = new List<Swipe>{
            new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow),
            new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Pass, DateTime.UtcNow)
        };
        _testDb.DbContext.Swipes.AddRange(swipes);
        _testDb.DbContext.SaveChanges();

        var swipe = await _repository.GetBySwipedBy(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.NotNull(swipe);
    }

    [Fact]
    public async Task when_swipes_with_given_id_exsits_swipe_exists_should_return_true()
    {
        var swipes = new List<Swipe>{
            new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow),
            new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Pass, DateTime.UtcNow)
        };
        _testDb.DbContext.Swipes.AddRange(swipes);
        _testDb.DbContext.SaveChanges();

        var swipe = await _repository.SwipeExists(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.True(swipe);
        swipe = await _repository.SwipeExists(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.True(swipe);
    }

    [Fact]
    public async Task when_no_swipes_match_get_by_swiped_by_swiped_who_should_return_empty_collection()
    {
        var swipes = new List<Swipe>{
            new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow),
            new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Pass, DateTime.UtcNow)
        };
        _testDb.DbContext.Swipes.AddRange(swipes);
        _testDb.DbContext.SaveChanges();

        var swipe = await _repository.GetBySwipedBy(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Null(swipe);
        swipe = await _repository.GetBySwipedBy(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Null(swipe);
        swipe = await _repository.GetBySwipedBy(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.Null(swipe);
    }

    [Fact]
    public async Task when_no_swipes_match_swipe_exists_should_return_false()
    {
        // SwipeExists
        var swipes = new List<Swipe>{
            new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow),
            new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Pass, DateTime.UtcNow)
        };
        _testDb.DbContext.Swipes.AddRange(swipes);
        _testDb.DbContext.SaveChanges();

        var swipe = await _repository.SwipeExists(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.False(swipe);
        swipe = await _repository.SwipeExists(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.False(swipe);
        swipe = await _repository.SwipeExists(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.False(swipe);
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