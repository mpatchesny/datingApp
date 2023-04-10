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
public class MessageRepositoryTests : IDisposable
{
    [Fact]
    public async void get_existing_messages_by_match_id_should_return_nonempty_collection()
    {
        var messages = await _repository.GetByMatchIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Single(messages);
    }

    [Fact]
    public async void get_nonexisting_message_by_match_id_should_return_empty_collection()
    {
        var messages = await _repository.GetByMatchIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Empty(messages);
    }

    [Fact]
    public async void delete_existing_message_by_id_should_succeed()
    {
        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(Guid.Parse("00000000-0000-0000-0000-000000000001")));
        Assert.Null(exception);
    }

    [Fact]
    public async void after_delete_message_get_messages_by_match_id_should_return_minus_one_elements()
    {
        var messageId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        await _repository.DeleteAsync(messageId);
        var messages = await _repository.GetByMatchIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Empty(messages);
    }

    [Fact]
    public async void delete_nonexisting_message_by_id_should_throw_exception()
    {
        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(Guid.Parse("00000000-0000-0000-0000-000000000002")));
        Assert.NotNull(exception);
    }

    [Fact]
    public async void add_message_should_succeed()
    {
        var message = new Message(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow);
        await _repository.AddAsync(message);
        var messages = await _repository.GetByMatchIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.NotNull(messages);
        Assert.Equal(2, messages.Count());
    }

    [Fact]
    public async void add_message_with_existing_id_should_throw_exception()
    {
        var message = new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(message));
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void after_add_message_get_messages_by_match_id_should_return_plus_one_elements()
    {
        var message = new Message(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow);
        await _repository.AddAsync(message);
        var messages = await _repository.GetByMatchIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.NotNull(messages);
        Assert.Equal(3, messages.Count());
    }

    // Arrange
    private readonly IMessageRepository _repository;
    private readonly TestDatabase _testDb;
    public MessageRepositoryTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();

        var message = new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow);
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