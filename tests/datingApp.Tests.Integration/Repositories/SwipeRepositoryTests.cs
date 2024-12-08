using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class SwipeRepositoryTests : IDisposable
{
    [Fact]
    public async Task add_swipe_should_succeed()
    {
        var swipe = IntegrationTestHelper.CreateSwipe(Guid.NewGuid(), Guid.NewGuid(), Like.Like);

        await _repository.AddAsync(swipe);
        var addedSwipe = await _testDb.CreateNewDbContext().Swipes.FirstOrDefaultAsync(x => x.SwipedById == swipe.SwipedById && x.SwipedWhoId == swipe.SwipedWhoId);
        Assert.True(swipe.IsEqualTo(addedSwipe));
    }

    [Fact]
    public async Task add_swipe_with_existing_id_throws_exception()
    {
        var swipe = await IntegrationTestHelper.CreateSwipeAsync(_testDb.CreateNewDbContext(), Guid.NewGuid(), Guid.NewGuid(), Like.Like);
        var badSwipe = new Swipe(swipe.SwipedById, swipe.SwipedWhoId, Like.Like, DateTime.UtcNow);

        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(badSwipe));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task given_swipe_exists_get_by_swiped_by_swiped_who_returns_exactly_two_swipes()
    {
        var swipe1 = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, Guid.NewGuid(), Guid.NewGuid(), Like.Like);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, swipe1.SwipedWhoId, swipe1.SwipedById, Like.Pass);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, swipe1.SwipedWhoId, Guid.NewGuid(), Like.Pass);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, Guid.NewGuid(), swipe1.SwipedById, Like.Pass);
        _dbContext.ChangeTracker.Clear();

        var swipes = await _repository.GetBySwipedBy(swipe1.SwipedById, swipe1.SwipedWhoId);
        Assert.Equal(2, swipes.Count());
    }

    [Fact]
    public async Task when_no_swipes_match_get_by_swiped_by_returns_empty_list()
    {
        var swipe1 = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, Guid.NewGuid(), Guid.NewGuid(), Like.Like);
        var swipe2 = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, Guid.NewGuid(), Guid.NewGuid(), Like.Pass);
        _dbContext.ChangeTracker.Clear();

        var swipes = await _repository.GetBySwipedBy(swipe2.SwipedById, swipe2.SwipedById);
        Assert.Empty(swipes);
        swipes = await _repository.GetBySwipedBy(swipe2.SwipedWhoId, swipe2.SwipedWhoId);
        Assert.Empty(swipes);
        swipes = await _repository.GetBySwipedBy(swipe1.SwipedById, swipe1.SwipedById);
        Assert.Empty(swipes);
        swipes = await _repository.GetBySwipedBy(swipe1.SwipedWhoId, swipe1.SwipedWhoId);
        Assert.Empty(swipes);
        swipes = await _repository.GetBySwipedBy(swipe1.SwipedById, Guid.NewGuid());
        Assert.Empty(swipes);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly ISwipeRepository _repository;
    
    public SwipeRepositoryTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _repository = new DbSwipeRepository(_dbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}