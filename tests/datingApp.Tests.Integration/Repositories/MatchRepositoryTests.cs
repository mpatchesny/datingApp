using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.DAL.Repositories.Specifications;
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
    public async void given_match_not_exists_get_match_by_id_should_return_null()
    {
        var match = await _repository.GetByIdAsync(Guid.NewGuid());
        Assert.Null(match);
    }

    [Fact]
    public async void given_match_exists_get_exsits_should_return_true()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var exists = await _repository.ExistsAsync(user1.Id, user2.Id);
        Assert.True(exists);
    }

    [Fact]
    public async void given_match_not_exists_get_exsits_should_return_false()
    {
        var exists = await _repository.ExistsAsync(Guid.NewGuid(), Guid.NewGuid());
        Assert.False(exists);
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
        var match = new Match(Guid.NewGuid(), user1.Id, user2.Id, false, false, null, DateTime.UtcNow);

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

        var match2 = new Match(match.Id, user1.Id, user3.Id, false, false, null, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.NotNull(exception);
    }

    [Fact]
    public async void add_match_with_existing_user_id_throws_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var match2 = new Match(Guid.NewGuid(), user1.Id, user2.Id, false, false, null, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.NotNull(exception);
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
    public async void given_specification_is_not_null_get_match_includes_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 10; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);

        _dbContext.ChangeTracker.Clear();
        var retrievedMatch = await _repository.GetByIdAsync(match.Id, new MatchWithMessagesSpecification());
        Assert.True(match.IsEqualTo(retrievedMatch));
    }

    [Fact]
    public async void get_match_respects_specification_get_message_before_date()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        var date1 = DateTime.UtcNow.AddHours(-1);
        var date2 = DateTime.UtcNow;
        for (int i = 0; i < 3; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, createdAt: date1));
        }
        for (int i = 0; i < 4; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, createdAt: date2));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var specification = new MatchWithMessagesSpecification().GetMessagesBeforeDate(date1);
        var retrievedMatch = await _repository.GetByIdAsync(match.Id, specification);
        Assert.Equal(3, retrievedMatch.Messages.Count());
    }

    [Fact]
    public async void get_match_respects_specification_get_messages_by_displayed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 3; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, isDisplayed: true));
        }
        for (int i = 0; i < 4; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, isDisplayed: false));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var specification1 = new MatchWithMessagesSpecification().GetMessagesByDisplayed(true);
        var retrievedMatch1 = await _repository.GetByIdAsync(match.Id, specification1);
        Assert.Equal(3, retrievedMatch1.Messages.Count());
        _dbContext.ChangeTracker.Clear();

        var specification2 = new MatchWithMessagesSpecification().GetMessagesByDisplayed(false);
        var retrievedMatch2 = await _repository.GetByIdAsync(match.Id, specification2);
        Assert.Equal(4, retrievedMatch2.Messages.Count());
    }

    [Fact]
    public async void get_match_respects_specification_get_message_by_id()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 10; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var specification = new MatchWithMessagesSpecification().GetMessageById(messages[5].Id);
        var retrievedMatch = await _repository.GetByIdAsync(match.Id, specification);
        Assert.Single(retrievedMatch.Messages);
        Assert.Equal(messages[5].Id, retrievedMatch.Messages.FirstOrDefault().Id);
    }

    [Fact]
    public async void get_match_respects_specification_set_message_fetch_limit()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 10; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, createdAt: DateTime.UtcNow.AddSeconds(i)));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var specification = new MatchWithMessagesSpecification().SetMessageFetchLimit(5);
        var retrievedMatch = await _repository.GetByIdAsync(match.Id, specification);
        Assert.Collection(retrievedMatch.Messages, 
            message => Assert.Equal(message.Id, messages[^1].Id),
            message => Assert.Equal(message.Id, messages[^2].Id),
            message => Assert.Equal(message.Id, messages[^3].Id),
            message => Assert.Equal(message.Id, messages[^4].Id),
            message => Assert.Equal(message.Id, messages[^5].Id)
            );
    }

    [Fact]
    public async void get_match_respects_specification_with_more_than_one_argument()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        var beforeDate = DateTime.UtcNow;
        for (int i = 0; i < 7; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, createdAt: beforeDate));
        }
        for (int i = 0; i < 8; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, isDisplayed: true, createdAt: beforeDate.AddSeconds(-i)));
        }
        for (int i = 0; i < 9; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, createdAt: beforeDate));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var specification = new MatchWithMessagesSpecification()
                                .GetMessagesBeforeDate(beforeDate)
                                .GetMessagesByDisplayed(true)
                                .SetMessageFetchLimit(5);
        var retrievedMatch = await _repository.GetByIdAsync(match.Id, specification);
        Assert.Equal(5, retrievedMatch.Messages.Count());
        Assert.Collection(retrievedMatch.Messages, 
            message => Assert.Equal(message.Id, messages[7].Id),
            message => Assert.Equal(message.Id, messages[8].Id),
            message => Assert.Equal(message.Id, messages[9].Id),
            message => Assert.Equal(message.Id, messages[10].Id),
            message => Assert.Equal(message.Id, messages[11].Id)
            );
    }

    // TODO: dodać testy żeby sprawdzić, czy UpdateAsync łapie zmiany w wiadomościach (AddMessage, RemoveMEssage itd.)

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