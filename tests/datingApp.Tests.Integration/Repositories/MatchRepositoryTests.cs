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


public class MatchRepositoryTests : IDisposable
{
    [Fact]
    public async void given_match_exists_get_match_by_user_id_should_return_users_matches_with_messages()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var matches = await _repository.GetByUserIdAsync(user1.Id);
        Assert.Single(matches);
        Assert.NotEmpty(matches.Select(x => x.Messages).ToList());
        Assert.Empty(matches.Where(x => !x.UserId1.Equals(user1.Id) && !x.UserId2.Equals(user1.Id)));
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
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var match2 = await _repository.GetByIdAsync(match.Id);
        Assert.NotNull(match);
        Assert.Equal(match, match2);
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
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

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
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(match));
        Assert.Null(exception);
        var deletedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.Null(deletedMatch);
    }

    [Fact]
    public async void after_delete_match_get_matches_by_user_id_should_return_minus_one_elements()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        await _repository.DeleteAsync(match);
        var matches = await _repository.GetByUserIdAsync(user1.Id);
        Assert.Empty(matches);
    }

    [Fact]
    public async void add_match_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = new Match(Guid.NewGuid(), user1.Id, user2.Id, false, false, null, DateTime.UtcNow);

        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.Null(exception);
        var addedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.Same(addedMatch, match);
    }

    [Fact]
    public async void update_match_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        match.SetDisplayed(match.UserId1);

        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateAsync(match));
        Assert.Null(exception);
        var updatedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == match.Id);
        Assert.Same(updatedMatch, match);
    }

    [Fact]
    public async void add_match_with_existing_id_throws_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var match2 = new Match(match.Id, user1.Id, user3.Id, false, false, null, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.NotNull(exception);
        // Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void add_match_with_existing_user_id_throws_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var match2 = new Match(Guid.NewGuid(), user1.Id, user2.Id, false, false, null, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.NotNull(exception);
        // Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void after_add_match_get_matches_by_user_id_should_return_plus_one_elements()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var match = new Match(Guid.NewGuid(), user1.Id, user3.Id, false, false, null, DateTime.UtcNow);
        await _repository.AddAsync(match);
        var matches = await _repository.GetByUserIdAsync(user1.Id);
        Assert.Equal(2, matches.Count());
    }

    [Fact]
    public async void get_match_by_nonexisting_message_id_returns_null()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var match = await _repository.GetByMessageIdAsync(Guid.NewGuid());
        Assert.Null(match);
    }

    [Fact]
    public async void get_match_by_existing_message_id_returns_match()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id, DateTime.UtcNow);

        var retrievedMatch = await _repository.GetByMessageIdAsync(message.Id);
        Assert.Same(match, retrievedMatch);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly IMatchRepository _repository;
    public MatchRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repository = new DbMatchRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}