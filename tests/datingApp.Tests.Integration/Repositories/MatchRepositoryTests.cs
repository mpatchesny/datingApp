using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.DAL.Specifications;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class MatchRepositoryTests : IDisposable
{
    [Fact]
    public async void given_match_exists_get_match_by_user_id_should_return_users_matches()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user4 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user3.Id);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user4.Id);

        var matches = await _repository.GetByUserIdAsync(user1.Id);
        Assert.Equal(3, matches.Count());
    }

    [Fact]
    public async void get_match_by_nonexisting_user_id_should_return_empty_collection()
    {
        var matches = await _repository.GetByUserIdAsync(Guid.NewGuid());
        Assert.Empty(matches);
    }

    [Fact]
    public async void given_match_exists_get_match_by_id_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);

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
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);

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
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);

        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(match));
        Assert.Null(exception);
        var deletedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.Null(deletedMatch);
    }

    [Fact]
    public async void after_delete_match_get_matches_by_user_id_should_return_minus_one_elements()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);

        await _repository.DeleteAsync(match);
        var matches = await _repository.GetByUserIdAsync(user1.Id);
        Assert.Empty(matches);
    }

    [Fact]
    public async void add_match_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = new Match(Guid.NewGuid(), user1.Id, user2.Id, false, false, null, DateTime.UtcNow);

        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.Null(exception);
        var addedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.True(addedMatch.IsEqualTo(match));
    }

    [Fact]
    public async void update_match_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);
        match.SetDisplayed(match.UserId1);

        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateAsync(match));
        Assert.Null(exception);
        var updatedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.True(updatedMatch.IsEqualTo(match));
    }

    [Fact]
    public async void add_match_with_existing_id_throws_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);

        var match2 = new Match(match.Id, user1.Id, user3.Id, false, false, null, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.NotNull(exception);
        // Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void add_match_with_existing_user_id_throws_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);

        var match2 = new Match(Guid.NewGuid(), user1.Id, user2.Id, false, false, null, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.NotNull(exception);
        // Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void after_add_match_get_matches_by_user_id_should_return_plus_one_elements()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);

        var match = new Match(Guid.NewGuid(), user1.Id, user3.Id, false, false, null, DateTime.UtcNow);
        await _repository.AddAsync(match);
        var matches = await _repository.GetByUserIdAsync(user1.Id);
        Assert.Equal(2, matches.Count());
    }

    [Fact]
    public async void get_match_by_nonexisting_message_id_returns_null()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);

        var match = await _repository.GetByMessageIdAsync(Guid.NewGuid());
        Assert.Null(match);
    }

    [Fact]
    public async void get_match_by_existing_message_id_returns_match()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);
        var message1 = await IntegrationTestHelper.CreateMessageAsync(_testDb.CreateNewDbContext(), match.Id, user1.Id, DateTime.UtcNow);
        var message2 = await IntegrationTestHelper.CreateMessageAsync(_testDb.CreateNewDbContext(), match.Id, user2.Id, DateTime.UtcNow);
        var message3 = await IntegrationTestHelper.CreateMessageAsync(_testDb.CreateNewDbContext(), match.Id, user1.Id, DateTime.UtcNow);

        foreach (var message in new Message[]{ message1, message2, message3 })
        {
            var retrievedMatch = await _repository.GetByMessageIdAsync(message.Id);
            Assert.True(match.IsEqualTo(retrievedMatch));
        }
    }

    [Fact]
    public async void given_specification_is_not_null_get_match_includes_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb.CreateNewDbContext());
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb.CreateNewDbContext(), user1.Id, user2.Id);
        for (int i = 0; i < 50; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb.CreateNewDbContext(), match.Id, user1.Id, DateTime.UtcNow);
        }

        var retrievedMatch = await _repository.GetByIdAsync(match.Id);
        Assert.True(match.IsEqualTo(retrievedMatch));
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly IMatchRepository _repository;
    public MatchRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repository = new DbMatchRepository(_testDb.CreateNewDbContext());
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}