using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetMessagesHandlerTests : IDisposable
{
    [Fact]
    public async Task query_messages_by_existing_match_id_should_return_nonempty_collection_of_messages_dto()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 5; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id, "test");
        }

        var query = new GetMessages() { MatchId = match.Id};
        var messages = await _handler.HandleAsync(query);
        Assert.NotEmpty(messages.Data);
        Assert.IsType<MessageDto>(messages.Data.First());
    }

    [Fact]
    public async Task query_messages_by_nonexisting_match_id_should_return_match_not_exists_exception()
    {
        var query = new GetMessages() { MatchId = Guid.NewGuid() };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    [Fact]
    public async Task returned_messages_count_is_lower_or_equal_to_page_size()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 10; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id, "test");
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(5);
        var messages = await _handler.HandleAsync(query);
        Assert.InRange(messages.Data.Count(), 0, query.PageSize);
    }

    [Fact]
    public async Task proper_number_of_messages_are_returned_when_page_is_above_1()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 9; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id, "test");
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(5);
        query.SetPage(2);
        var messages = await _handler.HandleAsync(query);
        Assert.NotEmpty(messages.Data);
        Assert.Equal(4, messages.Data.Count());
    }

    [Fact]
    public async Task paginated_data_dto_returns_proper_number_page_count()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 9; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id, "test");
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var messages = await _handler.HandleAsync(query);
        Assert.Equal(9, messages.PageCount);
    }

    [Fact]
    public async Task paginated_data_dto_returns_proper_number_of_page_size()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 9; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id, "test");
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(1, matches.PageSize);
    }

    [Fact]
    public async Task paginated_data_dto_returns_proper_page()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 9; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id, "test");
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(2);
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(2, matches.Page);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMessagesHandler _handler;
    public GetMessagesHandlerTests()
    {
        _testDb = new TestDatabase();
        _handler = new GetMessagesHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}