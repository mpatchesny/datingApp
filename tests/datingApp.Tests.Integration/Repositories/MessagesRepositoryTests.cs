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
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);
        
        var messages = await _repository.GetByMatchIdAsync(match.Id);
        Assert.Single(messages);
    }

    [Fact]
    public async void get_previous_not_displayed_messages_returns_not_displayed_messages_within_one_match_that_were_sent_before_the_message()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match1 = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var match2 = await IntegrationTestHelper.CreateMatchAsync(_testDb, user2.Id, user3.Id);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match1.Id, user1.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match1.Id, user1.Id);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match1.Id, user1.Id);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match2.Id, user1.Id);

        var result = await _repository.GetPreviousNotDisplayedMessages(message.Id);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async void get_existing_message_by_id_should_return_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);

        var returnedMessage = await _repository.GetByIdAsync(message.Id);
        Assert.Equal(message, returnedMessage);
    }

    [Fact]
    public async void given_message_not_exists_get_message_by_id_should_return_null()
    {
        var message = await _repository.GetByIdAsync(Guid.NewGuid());
        Assert.Null(message);
    }

    [Fact]
    public async void given_message_not_exists_get_message_by_match_id_should_return_empty_collection()
    {
        var messages = await _repository.GetByMatchIdAsync(Guid.NewGuid());
        Assert.Empty(messages);
    }

    [Fact]
    public async void delete_existing_message_by_id_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);

        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(message.Id));
        Assert.Null(exception);
        var deletedMessage = await _testDb.DbContext.Messages.FirstOrDefaultAsync(x => x.Id == message.Id);
        Assert.Null(deletedMessage);
    }

    [Fact]
    public async void after_delete_message_get_messages_by_match_id_should_return_minus_one_elements()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);

        await _repository.DeleteAsync(message.Id);
        var messages = await _repository.GetByMatchIdAsync(match.Id);
        Assert.Empty(messages);
    }

    [Fact]
    public async void given_message_not_exists_delete_message_by_id_should_not_throw_exception()
    {
        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(Guid.NewGuid()));
        Assert.NotNull(exception);
    }

    [Fact]
    public async void add_message_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var message = new Message(Guid.NewGuid(), match.Id, user1.Id, "ahoj", false, DateTime.UtcNow);
        await _repository.AddAsync(message);
        var addedMessage = _testDb.DbContext.Messages.FirstOrDefault(x => x.Id == message.Id);
        Assert.Same(message, addedMessage);
    }

    [Fact]
    public async void update_message_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);

        message.SetDisplayed();
        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateAsync(message));
        Assert.Null(exception);
        var updatedMessage = _testDb.DbContext.Messages.FirstOrDefault(x => x.Id == message.Id);
        Assert.Same(message, updatedMessage);
    }

    [Fact]
    public async void update_range_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var messages = new List<Message> {
            await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id),
            await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id),
            await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id),
            await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id),
        };

        foreach (var message in messages)
        {
            message.SetDisplayed();
        }

        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateRangeAsync(messages.ToArray()));
        Assert.Null(exception);
    }

    [Fact]
    public async void add_message_with_existing_id_throws_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);

        var badMessage = new Message(message.Id, match.Id, user1.Id, "ahoj", false, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(message));
        Assert.NotNull(exception);
    }

    [Fact]
    public async void after_add_message_get_messages_by_match_id_should_return_plus_one_elements()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);
        
        var newMessage = new Message(Guid.NewGuid(), match.Id, user1.Id, "ahoj", false, DateTime.UtcNow);
        await _repository.AddAsync(newMessage);
        
        var messages = await _repository.GetByMatchIdAsync(match.Id);
        Assert.NotNull(messages);
        Assert.Equal(2, messages.Count());
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly IMessageRepository _repository;
    public MessageRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repository = new DbMessageRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}