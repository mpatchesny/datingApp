using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.Spatial;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetSwipeCandidatesHandlerTests : IDisposable
{
    [Theory]
    [InlineData(PreferredSex.Male, UserSex.Male)]
    [InlineData(PreferredSex.Female, UserSex.Female)]
    [InlineData(PreferredSex.Male | PreferredSex.Female, UserSex.Female)]
    [InlineData(PreferredSex.Male | PreferredSex.Female, UserSex.Male)]
    public async Task when_candidates_with_proper_sex_exist_returns_nonempty_list(PreferredSex userLookingForSex, UserSex candidateSex)
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var settings1 = new UserSettings(Guid.NewGuid(), userLookingForSex, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user1 = new User(settings1.UserId, "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Male, null, settings1);

        var settings2 = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user2 = new User(settings2.UserId, "222222222", "test2@test.com", "Janusz", new DateOnly(2000,1,1), candidateSex, null, settings2);

        await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext(), user1);
        await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext(), user2);
        _dbContext.ChangeTracker.Clear();

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 999 };
        var candidates = await _handler.HandleAsync(query);
        Assert.Single(candidates);
    }

    [Theory]
    [InlineData(PreferredSex.Male, UserSex.Female)]
    [InlineData(PreferredSex.Female, UserSex.Male)]
    public async Task when_no_candidates_with_proper_sex_returns_empty_list(PreferredSex userLookingForSex, UserSex candidateSex)
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var settings1 = new UserSettings(Guid.NewGuid(), userLookingForSex, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user1 = new User(settings1.UserId, "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Male, null, settings1);

        var settings2 = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user2 = new User(settings2.UserId, "222222222", "test2@test.com", "Janusz", new DateOnly(2000,1,1), candidateSex, null, settings2);

        await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext(), user1);
        await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext(), user2);
        _dbContext.ChangeTracker.Clear();

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 999 };
        var candidates = await _handler.HandleAsync(query);
        Assert.Empty(candidates);
    }

    [Theory]
    [InlineData(18, 18, 18)]
    [InlineData(100, 100, 100)]
    [InlineData(18, 25, 18)]
    [InlineData(18, 25, 25)]
    [InlineData(18, 100, 45)]
    public async Task when_candidates_with_proper_age_exists_returns_nonempty_list(int queryAgeFrom, int queryAgeTo, int candidateAge)
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var settings1 = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(queryAgeFrom, queryAgeTo), 100, new Location(0.0, 0.0));
        var user1 = new User(settings1.UserId, "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Female, null, settings1);

        int nowYear = DateTime.UtcNow.Year, nowMonth = DateTime.UtcNow.Month, nowDay= DateTime.UtcNow.Day;
        var candidateDateOfBirth = new DateOnly(nowYear - candidateAge, nowMonth, nowDay);
        var settings2 = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user2 = new User(settings2.UserId, "222222222", "test2@test.com", "Janusz", candidateDateOfBirth, UserSex.Female, null, settings2);
        
        await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext(), user1);
        await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext(), user2);
        _dbContext.ChangeTracker.Clear();

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 999 };
        var candidates = await _handler.HandleAsync(query);
        Assert.Single(candidates);
    }

    [Theory]
    [InlineData(18, 18, 19)]
    [InlineData(100, 100, 99)]
    [InlineData(20, 25, 19)]
    [InlineData(20, 25, 26)]
    [InlineData(50, 60, 70)]
    public async Task when_no_candidates_with_proper_age_get_swipe_candidates_returns_empty_list(int queryAgeFrom, int queryAgeTo, int candidateAge)
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var settings1 = new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(queryAgeFrom, queryAgeTo), 100, new Location(0.0, 0.0));
        var user1 = new User(settings1.UserId, "111111111", "test@test.com", "Janusz", new DateOnly(2000, 1, 1), UserSex.Male, null, settings1);

        int nowYear = DateTime.UtcNow.Year, nowMonth = DateTime.UtcNow.Month, nowDay= DateTime.UtcNow.Day;
        var candidateDateOfBirth = new DateOnly(nowYear - candidateAge, nowMonth, nowDay);
        var settings2 = new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user2 = new User(settings2.UserId, "222222222", "test2@test.com", "Janusz", candidateDateOfBirth, UserSex.Male, null, settings2);

        await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext(), user1);
        await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext(), user2);
        _dbContext.ChangeTracker.Clear();

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 999 };
        var candidates = await _handler.HandleAsync(query);
        Assert.Empty(candidates);
    }

    [Fact]
    public async Task when_candidates_within_range_exist_get_swipe_candidates_returns_nonempty_list()
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 999 };
        var candidates = await _handler.HandleAsync(query);
        Assert.Single(candidates);
    }

    [Fact]
    public async Task when_no_candidates_within_range_get_swipe_candidates_returns_empty_list()
    {
        _spatial.Setup(m => m.CalculateDistanceInKms(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(999);
        var mockedCoordsResult = new Coords(northLat: 100.0d, southLat: -100.0d, eastLon: 100.0d, westLon: -100.0d);
        _spatial.Setup(m => m.GetApproxSquareAroundPoint(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>()))
            .Returns((double x, double y, int z) => mockedCoordsResult);

        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 999 };
        var candidates = await _handler.HandleAsync(query);
        Assert.Empty(candidates);
    }

    [Fact]
    public async Task get_swipe_candidates_returns_candidates_sorted_by_number_of_likes_descending()
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user4 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user5 = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb.CreateNewDbContext(), user2.Id, user5.Id, Like.Like);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb.CreateNewDbContext(), user3.Id, user5.Id, Like.Like);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb.CreateNewDbContext(), user4.Id, user5.Id, Like.Like);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb.CreateNewDbContext(), user2.Id, user4.Id, Like.Like);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb.CreateNewDbContext(), user3.Id, user4.Id, Like.Like);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb.CreateNewDbContext(), user2.Id, user3.Id, Like.Like);
        _dbContext.ChangeTracker.Clear();

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 999 };
        var candidates = await _handler.HandleAsync(query);
        Assert.NotEmpty(candidates);
        Assert.Equal(candidates.ToList()[0].Id, user5.Id.Value);
        Assert.Equal(candidates.ToList()[1].Id, user4.Id.Value);
        Assert.Equal(candidates.ToList()[2].Id, user3.Id.Value);
    }

    [Fact]
    public async Task get_swipe_candidates_not_returns_users_already_swiped()
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id, Like.Like);
        _dbContext.ChangeTracker.Clear();

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 2 };
        var candidates = await _handler.HandleAsync(query);
        Assert.Single(candidates);
    }

    [Fact]
    public async Task get_swipe_candidates_returns_candidates_count_equals_to_how_many()
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var query = new GetSwipeCandidates() { UserId = user1.Id, HowMany = 2};
        var candidates = await _handler.HandleAsync(query);
        Assert.Equal(query.HowMany, candidates.Count());
    }

    [Fact]
    public async Task given_user_who_requested_not_exists_get_swipe_candidates_returns_UserNotExistsExceptionn()
    {
        SetMockedSpatialDefaultReturnValues(_spatial);
        var query = new GetSwipeCandidates() { UserId = Guid.NewGuid(), HowMany = 2 };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly Mock<ISpatial> _spatial;
    private readonly GetSwipeCandidatesHandler _handler;
    public GetSwipeCandidatesHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _spatial = new Mock<ISpatial>();
        _handler = new GetSwipeCandidatesHandler(_testDb.DbContext, _spatial.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }

    private void SetMockedSpatialDefaultReturnValues(Mock<ISpatial> spatial)
    {
        spatial.Setup(m => m.CalculateDistanceInKms(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(0);
        var mockedCoordsResult = new Coords(northLat: 100.0d, southLat: -100.0d, eastLon: 100.0d, westLon: -100.0d);
        spatial.Setup(m => m.GetApproxSquareAroundPoint(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>()))
            .Returns((double x, double y, int z) => mockedCoordsResult);
    }
}