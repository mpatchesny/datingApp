using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class DeleteMatchHandlerTests : IDisposable
{
    [Fact]
    public async Task delete_existing_match_should_succeed_and_add_deleted_match_id_to_deleted_entities()
    {
        var matchId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var command = new DeleteMatch(matchId);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        Assert.True(await _testDb.DbContext.DeletedEntities.AnyAsync(x => x.Id == matchId));
    }
    
    [Fact]
    public async Task delete_nonexisting_match_should_throw_exception()
    {
        var command = new DeleteMatch(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_match_id_exists_in_deleted_entities_repository_delete_user_should_throw_already_deleted_exception()
    {
        var alreadyDeletedMatch = new DeletedEntityDto()
        {
            Id = Guid.NewGuid()
        };
        await _testDb.DbContext.DeletedEntities.AddAsync(alreadyDeletedMatch);
        await _testDb.DbContext.SaveChangesAsync();
        var command = new DeleteMatch(alreadyDeletedMatch.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<MatchAlreadyDeletedException>(exception);
    }
    
    // Arrange
    private readonly DeleteMatchHandler _handler;
    private readonly TestDatabase _testDb;
    public DeleteMatchHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111111", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();

        var matchRepository = new PostgresMatchRepository(_testDb.DbContext);
        var deletedEntitiesRepository = new PostgresDeletedEntityRepository(_testDb.DbContext);
        _handler = new DeleteMatchHandler(matchRepository, deletedEntitiesRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}