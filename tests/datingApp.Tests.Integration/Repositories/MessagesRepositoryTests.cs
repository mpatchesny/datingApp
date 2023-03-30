using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;

public class MessageRepositoryTests : IDisposable
{
    [Fact]
    public async void get_existing_messages_by_match_id_should_succeed()
    {
        var messages = await _repository.GetByMatchIdAsync(1);
        Assert.Equal(1, messages.Count());
    }

    [Fact]
    public async void get_nonexisting_message_by_match_id_should_return_empty_collection()
    {
        var messages = await _repository.GetByMatchIdAsync(1);
        Assert.Equal(0, messages.Count());
    }

    [Fact]
    public async void delete_existing_message_by_id_should_succeed()
    {
        var messageId = 1;
        await _repository.DeleteAsync(messageId);
        _testDb.DbContext.SaveChanges();
        var matches = await _repository.GetByMatchIdAsync(1);
        Assert.Equal(0, matches.Count());
    }

    [Fact]
    public async void add_message_should_succeed()
    {
        var message = new Message(0, 1, 1, 1, "ahoj", false, DateTime.UtcNow);
        await _repository.AddAsync(message);
        _testDb.DbContext.SaveChanges();
        var messages = await _repository.GetByMatchIdAsync(1);
        Assert.NotNull(messages);
        Assert.Equal(2, messages.Count());
    }

    // Arrange
    private readonly IMessageRepository _repository;
    private readonly TestDatabase _testDb;
    public MessageRepositoryTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var match = new Match(0, 1, 1, null, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();

        var message = new Message(0, 1, 1, 1, "ahoj", false, DateTime.UtcNow);
        _testDb.DbContext.Messages.Add(message);
        _testDb.DbContext.SaveChanges();

        _repository = new PostgresMessageRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}