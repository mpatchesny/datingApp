using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.Spatial;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;

[Collection("Integration tests")]
public class GetSwipeCandidatesHandlerTests : IDisposable
{
    [Theory]
    [InlineData(Sex.Male, Sex.Male)]
    [InlineData(Sex.Female, Sex.Female)]
    [InlineData(Sex.Male | Sex.Female, Sex.Female)]
    [InlineData(Sex.Male | Sex.Female, Sex.Male)]
    public async Task when_candidates_with_proper_sex_exist_returns_nonempty_list(Sex userLookingForSex, Sex candidateSex)
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", new DateOnly(2000,1,1), candidateSex, null, settings2);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = 18;
        query.AgeTo = 100;
        query.Range = 100;
        query.Lat = 0.0;
        query.Lon = 0.0;
        query.Sex = (int) userLookingForSex;
        query.HowMany = 2;

        var candidates = await _handler.HandleAsync(query);
        Assert.Single(candidates);
    }

    [Theory]
    [InlineData(Sex.Male, Sex.Female)]
    [InlineData(Sex.Female, Sex.Male)]
    public async Task when_no_candidates_with_proper_sex_returns_empty_list(Sex userLookingForSex, Sex candidateSex)
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", new DateOnly(2000,1,1), candidateSex, null, settings2);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = 18;
        query.AgeTo = 100;
        query.Range = 100;
        query.Lat = 0.0;
        query.Lon = 0.0;
        query.Sex = (int) userLookingForSex;
        query.HowMany = 2;

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
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Male, 18, 21, 20, 0.0, 0.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Male, 18, 21, 20, 0.0, 0.0);
        var now = DateTime.UtcNow;
        var dob = new DateOnly(now.Year - candidateAge, now.Month, now.Day);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", dob, Sex.Male, null, settings2);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = queryAgeFrom;
        query.AgeTo = queryAgeTo;
        query.Range = 100;
        query.Lat = 0.0;
        query.Lon = 0.0;
        query.Sex = (int) Sex.Male;
        query.HowMany = 2;

        var candidates = await _handler.HandleAsync(query);
        Assert.Single(candidates);
    }

    [Theory]
    [InlineData(18, 18, 19)]
    [InlineData(100, 100, 99)]
    [InlineData(20, 25, 19)]
    [InlineData(20, 25, 26)]
    [InlineData(50, 60, 70)]
    public async Task when_no_candidates_with_proper_age_returns_empty_list(int queryAgeFrom, int queryAgeTo, int candidateAge)
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Male, 18, 21, 20, 0.0, 0.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Male, 18, 21, 20, 0.0, 0.0);
        var now = DateTime.UtcNow;
        var dob = new DateOnly(now.Year - candidateAge, now.Month, now.Day);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", dob, Sex.Male, null, settings2);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = queryAgeFrom;
        query.AgeTo = queryAgeTo;
        query.Range = 100;
        query.Lat = 0.0;
        query.Lon = 0.0;
        query.Sex = (int) Sex.Male;
        query.HowMany = 2;

        var candidates = await _handler.HandleAsync(query);
        Assert.Empty(candidates);
    }

    [Fact]
    public async Task when_candidates_within_range_exist_returns_nonempty_list()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Male, 18, 21, 20, 0.0, 0.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Male, 18, 21, 20, 0.0, 0.0);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings2);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = 18;
        query.AgeTo = 100;
        query.Range = 25;
        query.Lat = 0.0;
        query.Lon = 0.0;
        query.Sex = (int) Sex.Male;
        query.HowMany = 2;

        var candidates = await _handler.HandleAsync(query);
        Assert.Single(candidates);
    }

    [Fact]
    public async Task when_no_candidates_within_range_returns_empty_list()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Male, 18, 21, 20, 1.0, 1.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Male, 18, 21, 20, 1.0, 1.0);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings2);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = 18;
        query.AgeTo = 100;
        query.Range = 25;
        query.Lat = 1.0;
        query.Lon = 1.0;
        query.Sex = (int) Sex.Male;
        query.HowMany = 2;

        var candidates = await _handler.HandleAsync(query);
        Assert.Empty(candidates);
    }

    [Fact]
    public async Task candidates_returned_are_sorted_by_number_of_likes_descending()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Male, 18, 21, 20, 1.0, 1.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Male, 18, 21, 20, 1.0, 1.0);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings2);
        
        var settings3 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000003"), Sex.Male, 18, 21, 20, 1.0, 1.0);
        var user3 = new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "333333333", "test3@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings3);

        var settings4 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000004"), Sex.Male, 18, 21, 20, 1.0, 1.0);
        var user4 = new User(Guid.Parse("00000000-0000-0000-0000-000000000004"), "444444444", "test4@test.com", "Janusz", new DateOnly(2000, 1, 1), Sex.Male, null, settings4);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.Users.Add(user3);
        _testDb.DbContext.Users.Add(user4);
        _testDb.DbContext.SaveChanges();

        var swipes = new List<Swipe>{
            new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000004"), Like.Like, DateTime.UtcNow),
            new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000004"), Like.Like, DateTime.UtcNow),
            new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000004"), Like.Like, DateTime.UtcNow),
            new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000004"), Like.Like, DateTime.UtcNow),
            new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000004"), Like.Like, DateTime.UtcNow),
            new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow),
            new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000003"), Like.Pass, DateTime.UtcNow),
            new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000003"), Like.Pass, DateTime.UtcNow)
        };
        _testDb.DbContext.Swipes.AddRange(swipes);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = 1;
        query.AgeTo = 100;
        query.Range = 100;
        query.Lat = 0.0;
        query.Lon = 0.0;
        query.Sex = 3;
        query.HowMany = 999;

        var candidates = await _handler.HandleAsync(query);
        Assert.NotEmpty(candidates);
        Assert.Equal(candidates.ToList()[0].Id, user4.Id);
        Assert.Equal(candidates.ToList()[1].Id, user2.Id);
        Assert.Equal(candidates.ToList()[2].Id, user3.Id);
    }

    [Fact]
    public async Task users_already_swipped_are_not_returned()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);

        var settings3 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000003"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user3 = new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "333333333", "test3@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings3);

        var swipe = new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.Users.Add(user3);
        _testDb.DbContext.Swipes.Add(swipe);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = 18;
        query.AgeTo = 100;
        query.Range = 100;
        query.Lat = 0.0;
        query.Lon = 0.0;
        query.Sex = 1;
        query.HowMany = 2;

        var candidates = await _handler.HandleAsync(query);
        Assert.Single(candidates);
    }

    [Fact]
    public async Task candidates_returned_count_equals_how_many_from_query()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "222222222", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);

        var settings3 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000003"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user3 = new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "333333333", "test3@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings3);

        var settings4 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000004"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user4 = new User(Guid.Parse("00000000-0000-0000-0000-000000000004"), "444444444", "test4@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings4);

        var settings5 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000005"), Sex.Female, 18, 21, 20, 0.0, 0.0);
        var user5 = new User(Guid.Parse("00000000-0000-0000-0000-000000000005"), "555555555", "test5@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings5);

        var swipe = new Swipe(0, Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Like.Like, DateTime.UtcNow);

        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.Users.Add(user3);
        _testDb.DbContext.Users.Add(user4);
        _testDb.DbContext.Users.Add(user5);
        _testDb.DbContext.Swipes.Add(swipe);
        _testDb.DbContext.SaveChanges();

        var query = new GetSwipeCandidates();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.AgeFrom = 18;
        query.AgeTo = 100;
        query.Range = 100;
        query.Lat = 0.0;
        query.Lon = 0.0;
        query.Sex = 1;
        query.HowMany = 2;

        var candidates = await _handler.HandleAsync(query);
        Assert.Equal(query.HowMany, candidates.Count());
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetSwipeCandidatesHandler _handler;
    public GetSwipeCandidatesHandlerTests()
    {
        _testDb = new TestDatabase();
        var mockedSpatial = new Mock<ISpatial>();
        mockedSpatial.Setup(m => m.CalculateDistance(0.0, 0.0, 0.0, 0.0)).Returns(25);
        mockedSpatial.Setup(m => m.CalculateDistance(1.0, 1.0, 1.0, 1.0)).Returns(26);

        double north = (2.0);
        double south = (-1.0);
        double east = ( 2.0);
        double west = (-1.0);
        var mockedCoordsResult = new Coords(north, south, east, west);

        mockedSpatial.Setup(m => m.GetApproxSquareAroundPoint(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>()))
            .Returns((double x, double y, int z) => mockedCoordsResult);
        _handler = new GetSwipeCandidatesHandler(_testDb.DbContext, mockedSpatial.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}