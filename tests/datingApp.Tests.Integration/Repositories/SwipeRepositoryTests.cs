using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
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
        var swipe = new Swipe(Guid.NewGuid(), Guid.NewGuid(), Like.Like, DateTime.UtcNow);

        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(swipe));
        Assert.Null(exception);
        var addedSwipe = await _testDb.DbContext.Swipes.FirstOrDefaultAsync(x => x.SwipedById == swipe.SwipedById && x.SwipedWhoId == swipe.SwipedWhoId);
        Assert.Same(swipe, addedSwipe);
    }

    [Fact]
    public async Task add_swipe_with_existing_id_should_throw_exception()
    {
        var swipe = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Like);
        var swipe2 = new Swipe(swipe.SwipedById, swipe.SwipedWhoId, Like.Like, DateTime.UtcNow);

        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(swipe2));
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task get_by_swiped_by_swiped_who_should_return_nonempty_collection()
    {
        // TODO: fix test name
        var swipe1 = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Like);
        var swipe2 = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Pass);

        var swipe = await _repository.GetBySwipedBy(swipe1.SwipedById, swipe1.SwipedWhoId);
        Assert.NotNull(swipe);
    }

    [Fact]
    public async Task when_swipes_with_given_id_exsits_swipe_exists_should_return_true()
    {
        var swipe1 = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Like);
        var swipe2 = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Pass);

        var swipe = await _repository.SwipeExists(swipe1.SwipedById, swipe1.SwipedWhoId);
        Assert.True(swipe);
        swipe = await _repository.SwipeExists(swipe2.SwipedById, swipe2.SwipedWhoId);
        Assert.True(swipe);
    }

    [Fact]
    public async Task when_no_swipes_match_get_by_swiped_by_swiped_who_should_return_empty_collection()
    {
        var swipe1 = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Like);
        var swipe2 = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Pass);

        var swipe = await _repository.GetBySwipedBy(swipe2.SwipedById, swipe2.SwipedById);
        Assert.Null(swipe);
        swipe = await _repository.GetBySwipedBy(swipe2.SwipedWhoId, swipe2.SwipedWhoId);
        Assert.Null(swipe);
        swipe = await _repository.GetBySwipedBy(swipe1.SwipedById, swipe1.SwipedById);
        Assert.Null(swipe);
        swipe = await _repository.GetBySwipedBy(swipe1.SwipedWhoId, swipe1.SwipedWhoId);
        Assert.Null(swipe);
        swipe = await _repository.GetBySwipedBy(swipe1.SwipedById, Guid.NewGuid());
        Assert.Null(swipe);
    }

    [Fact]
    public async Task when_no_swipes_match_swipe_exists_should_return_false()
    {
        var swipe1 = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Like);
        var swipe2 = await IntegrationTestHelper.CreateSwipeAsync(_testDb, Guid.NewGuid(), Guid.NewGuid(), Like.Pass);

        var swipe = await _repository.SwipeExists(swipe2.SwipedById, swipe2.SwipedById);
        Assert.False(swipe);
        swipe = await _repository.SwipeExists(swipe2.SwipedWhoId, swipe2.SwipedWhoId);
        Assert.False(swipe);
        swipe = await _repository.SwipeExists(swipe1.SwipedById, swipe1.SwipedById);
        Assert.False(swipe);
        swipe = await _repository.SwipeExists(swipe1.SwipedWhoId, swipe1.SwipedWhoId);
        Assert.False(swipe);
        swipe = await _repository.SwipeExists(swipe1.SwipedById, Guid.NewGuid());
        Assert.False(swipe);
    }

    // Arrange
    private readonly ISwipeRepository _repository;
    private readonly TestDatabase _testDb;
    
    public SwipeRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repository = new DbSwipeRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}