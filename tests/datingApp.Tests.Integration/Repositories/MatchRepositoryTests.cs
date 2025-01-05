using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class MatchRepositoryTests : IDisposable
{
    [Fact]
    public async void given_match_exists_get_match_by_id_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var retrievedMatch = await _repository.GetByIdAsync(match.Id);
        Assert.NotNull(match);
        Assert.True(match.IsEqualTo(retrievedMatch));
    }

    [Fact]
    public async void given_match_not_exists_get_match_by_user_id_should_return_null()
    {
        var match = await _repository.GetByIdAsync(Guid.NewGuid());
        Assert.Null(match);
    }

    [Fact]
    public async void delete_existing_match_by_id_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);

        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(match));
        Assert.Null(exception);
        _dbContext.ChangeTracker.Clear();
        var deletedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.Null(deletedMatch);
    }

    [Fact]
    public async void add_match_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = new Match(Guid.NewGuid(), user1.Id, user2.Id, DateTime.UtcNow);

        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.Null(exception);
        _dbContext.ChangeTracker.Clear();
        var addedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.True(addedMatch.IsEqualTo(match));
    }

    [Fact]
    public async void update_match_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        match.SetDisplayed(match.UserId1);

        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateAsync(match));
        Assert.Null(exception);
        _dbContext.ChangeTracker.Clear();
        var updatedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.True(updatedMatch.IsEqualTo(match));
    }

    [Fact]
    public async void add_match_with_existing_id_throws_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var match2 = new Match(match.Id, user1.Id, user3.Id, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match2));
        Assert.NotNull(exception);
    }

    [Fact]
    public async void add_match_with_existing_user_ids_throws_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var match2 = new Match(Guid.NewGuid(), user1.Id, user2.Id, DateTime.UtcNow);
        var exception1 = await Record.ExceptionAsync(async () => await _repository.AddAsync(match2));
        Assert.NotNull(exception1);

        var match3 = new Match(Guid.NewGuid(), user2.Id, user1.Id, DateTime.UtcNow);
        var exception2 = await Record.ExceptionAsync(async () => await _repository.AddAsync(match2));
        Assert.NotNull(exception2);
    }

    [Fact]
    public async void get_match_by_nonexisting_message_id_returns_null()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var match = await _repository.GetByMessageIdAsync(Guid.NewGuid());
        Assert.Null(match);
    }

    [Fact]
    public async void get_match_by_existing_message_id_returns_match()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() {
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user2.Id, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        foreach (var message in messages)
        {
            _dbContext.ChangeTracker.Clear();
            var retrievedMatch = await _repository.GetByMessageIdAsync(message.Id);
            Assert.True(match.Id == retrievedMatch.Id);
        }
    }

    [Fact]
    public async void given_no_includeMessage_specification_provided_get_match_by_id_returns_match_without_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() {
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user2.Id, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var retrievedMatch = await _repository.GetByIdAsync(match.Id);
        Assert.Empty(retrievedMatch.Messages);
    }

    [Fact]
    public async void given_no_includeMessage_specification_provided_get_match_by_message_id_returns_match_without_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() {
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user2.Id, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var retrievedMatch = await _repository.GetByMessageIdAsync(messages[0].Id);
        Assert.Empty(retrievedMatch.Messages);
    }

    [Fact]
    public async void given_includeMessage_specification_provided_get_match_by_id_returns_match_with_specified_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() {
            IntegrationTestHelper.CreateMessage(user2.Id, isDisplayed: false, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user2.Id, isDisplayed: false, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user2.Id, isDisplayed: true, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user1.Id, isDisplayed: false, createdAt: DateTime.UtcNow),
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var retrievedMatch = await _repository.GetByIdAsync(match.Id, match => 
            match.Messages.Where(msg => msg.IsDisplayed == false && !msg.SendFromId.Equals(user1.Id)));
        Assert.Equal(2, retrievedMatch.Messages.Count());
    }

    [Fact]
    public async Task given_match_message_is_added_update_match_should_update_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() {
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);

        var newMessage = IntegrationTestHelper.CreateMessage(user1.Id, text: "test2", createdAt: DateTime.UtcNow.AddSeconds(10));
        match.AddMessage(newMessage);
        await _repository.UpdateAsync(match);
        
        _dbContext.ChangeTracker.Clear();
        var updatedMatch = await _dbContext.Matches.Include(m => m.Messages).FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.True(match.IsEqualTo(updatedMatch));
    }

    [Fact]
    public async Task given_match_message_is_removed_update_match_should_update_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() {
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow.AddSeconds(10)),
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        match.RemoveMessage(messages[1].Id);

        await _repository.UpdateAsync(match);
        
        _dbContext.ChangeTracker.Clear();
        var updatedMatch = await _dbContext.Matches.Include(m => m.Messages).FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.True(match.IsEqualTo(updatedMatch));
    }

    [Fact]
    public async Task given_match_messages_is_set_as_displayed_update_match_should_update_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() {
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow),
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow.AddSeconds(-1)),
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow.AddSeconds(-1)),
            IntegrationTestHelper.CreateMessage(user1.Id, isDisplayed: true, createdAt: DateTime.UtcNow.AddSeconds(-1)),
            IntegrationTestHelper.CreateMessage(user1.Id, isDisplayed: true, createdAt: DateTime.UtcNow.AddSeconds(-1)),
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        match.SetPreviousMessagesAsDisplayed(messages[0].Id, user2.Id);

        await _repository.UpdateAsync(match);
        
        _dbContext.ChangeTracker.Clear();
        var updatedMatch = await _dbContext.Matches.Include(m => m.Messages).FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.True(match.IsEqualTo(updatedMatch));
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly IMatchRepository _repository;
    public MatchRepositoryTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _repository = new DbMatchRepository(_dbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}