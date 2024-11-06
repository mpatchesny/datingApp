using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetPrivateUserHandlerTests : IDisposable
{
    [Fact]
    public async Task given_user_exists_get_private_user_returns_private_user_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var query = new GetPrivateUser() {UserId = user.Id};
        var userDto = await _handler.HandleAsync(query);
        Assert.NotNull(userDto);
        Assert.IsType<PrivateUserDto>(userDto);
    }

    [Fact]
    public async Task given_user_not_exists_get_private_user_returns_UserNotExistsException()
    {
        var query = new GetPrivateUser() {UserId = Guid.NewGuid()};
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly GetPrivateUserHandler _handler;
    public GetPrivateUserHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _handler = new GetPrivateUserHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}