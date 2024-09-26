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


public class MessageRepositoryTests : IDisposable
{
    [Fact]
    public async void get_existing_messages_by_match_id_should_return_nonempty_collection()
    {
        var messages = await _repository.GetByMatchIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Single(messages);
    }

    [Fact]
    public async void get_previous_not_displayed_messages_should_return_previous_messages_with_same_match_that_are_not_displayed()
    {
        var messages = new List<Message>{
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", true, DateTime.UtcNow - TimeSpan.FromSeconds(1)),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow + TimeSpan.FromSeconds(1)),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000005"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow - TimeSpan.FromSeconds(10))
        };
        await _testDb.DbContext.Messages.AddRangeAsync(messages);
        await _testDb.DbContext.SaveChangesAsync();

        var result = await _repository.GetPreviousNotDisplayedMessages(Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async void get_existing_message_by_id_should_return_nonempty_message()
    {
        var message = await _repository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.NotNull(message);
    }

    [Fact]
    public async void get_nonexisting_message_by_id_should_return_null()
    {
        var message = await _repository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000000"));
        Assert.Null(message);
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
        var deletedMessage = await _testDb.DbContext.Messages.FirstOrDefaultAsync(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Null(deletedMessage);
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
        var addedMessage = _testDb.DbContext.Messages.FirstOrDefault(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Same(message, addedMessage);
    }

    [Fact]
    public async void update_message_should_succeed()
    {
        var message = await _repository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        message.SetDisplayed();
        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateAsync(message));
        Assert.Null(exception);
        var updatedMessage = _testDb.DbContext.Messages.FirstOrDefault(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Same(message, updatedMessage);
    }

    [Fact]
    public async void update_range_should_succeed()
    {
        var messages = new List<Message> {
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000005"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow)
        };
        await _testDb.DbContext.Messages.AddRangeAsync(messages);
        _testDb.DbContext.SaveChanges();

        var updatedMessages = await _repository.GetByMatchIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        foreach (var message in updatedMessages)
        {
            message.SetDisplayed();
        }

        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateRangeAsync(updatedMessages.ToArray()));
        Assert.Null(exception);
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
        Assert.Equal(2, messages.Count());
    }

    // Arrange
    private readonly IMessageRepository _repository;
    private readonly TestDatabase _testDb;
    public MessageRepositoryTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), PreferredSex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);
        var match2 = new Match(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.Matches.Add(match2);
        _testDb.DbContext.SaveChanges();

        var message = new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "ahoj", false, DateTime.UtcNow);
        _testDb.DbContext.Messages.Add(message);
        _testDb.DbContext.SaveChanges();

        _repository = new DbMessageRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}