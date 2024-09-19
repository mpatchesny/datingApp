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


public class GetPrivateUserHandlerTests : IDisposable
{
    [Fact]
    public async Task query_existing_user_should_return_private_user_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var query = new GetPrivateUser() {UserId = user.Id};
        var userDto = await _handler.HandleAsync(query);
        Assert.NotNull(userDto);
        Assert.IsType<PrivateUserDto>(userDto);
    }

    [Fact]
    public async Task query_nonexisting_user_should_return_user_not_exists_exception()
    {
        var query = new GetPrivateUser() {UserId = Guid.NewGuid()};
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetPrivateUserHandler _handler;
    public GetPrivateUserHandlerTests()
    {
        _testDb = new TestDatabase();
        _handler = new GetPrivateUserHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}