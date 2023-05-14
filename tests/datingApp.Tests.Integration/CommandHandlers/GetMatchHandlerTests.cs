using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

[Collection("Integration tests")]
public class GetMatchHandlerTests : IDisposable
{
    [Fact]
    public async Task given_match_exists_get_match_handler_should_return_match_dto_with_non_empty_user()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb.DbContext.Users.Add(user);
        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "123456799", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        await _testDb.DbContext.SaveChangesAsync();

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001") ,Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        await _testDb.DbContext.SaveChangesAsync();

        var handler = new GetMatchHandler(_testDb.DbContext);
        var query = new GetMatch{ MatchId = Guid.Parse("00000000-0000-0000-0000-000000000001"), UserId = Guid.Parse("00000000-0000-0000-0000-000000000001")};
        var result = await handler.HandleAsync(query);
        Assert.NotNull(result);
        Assert.IsType<MatchDto>(result);
        Assert.NotNull(result.User);
    }

    [Fact]
    public async Task given_match_exists_and_user_has_photos_get_match_handler_should_return_match_dto_with_non_empty_user_photos()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb.DbContext.Users.Add(user);
        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "123456799", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"),Guid.Parse("00000000-0000-0000-0000-000000000001"),"test","test", 0);
        _testDb.DbContext.Photos.Add(photo);
        await _testDb.DbContext.SaveChangesAsync();

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001") ,Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        await _testDb.DbContext.SaveChangesAsync();

        var handler = new GetMatchHandler(_testDb.DbContext);
        var query = new GetMatch{ MatchId = Guid.Parse("00000000-0000-0000-0000-000000000001"), UserId = Guid.Parse("00000000-0000-0000-0000-000000000001")};
        var result = await handler.HandleAsync(query);
        Assert.NotNull(result);
        Assert.Single(result.User.Photos);
        Assert.IsType<PhotoDto>(result.User.Photos.FirstOrDefault());
    }

    [Fact]
    public async Task given_match_exists_and_match_has_messages_get_match_handler_should_return_match_dto_with_non_empty_messages()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb.DbContext.Users.Add(user);
        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "123456799", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        await _testDb.DbContext.SaveChangesAsync();

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001") ,Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        await _testDb.DbContext.SaveChangesAsync();

        var message = new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "test", false, DateTime.UtcNow);
        _testDb.DbContext.Messages.Add(message);
        await _testDb.DbContext.SaveChangesAsync();

        var handler = new GetMatchHandler(_testDb.DbContext);
        var query = new GetMatch{ MatchId = Guid.Parse("00000000-0000-0000-0000-000000000001"), UserId = Guid.Parse("00000000-0000-0000-0000-000000000001")};
        var result = await handler.HandleAsync(query);
        Assert.NotNull(result);
        Assert.Single(result.Messages);
        Assert.IsType<MessageDto>(result.Messages.FirstOrDefault());
    }

    [Fact]
    public async Task given_match_not_exists_get_match_handler_should_return_null()
    {
        var handler = new GetMatchHandler(_testDb.DbContext);
        var query = new GetMatch{ MatchId = Guid.Parse("00000000-0000-0000-0000-000000000001"), UserId = Guid.Parse("00000000-0000-0000-0000-000000000001")};
        var result = await handler.HandleAsync(query);
        Assert.Null(result);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    public GetMatchHandlerTests()
    {
        _testDb = new TestDatabase();
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}