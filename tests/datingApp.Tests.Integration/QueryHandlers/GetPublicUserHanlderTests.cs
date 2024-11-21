using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Application.Spatial;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.Spatial;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;

public class GetPublicUserHanlderTests : IDisposable
{
    [Fact]
    public async Task given_users_exists_and_are_in_a_match_get_public_user_returns_public_user_dto()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var query = new GetPublicUser() { RequestByUserId = user1.Id, RequestWhoUserId = user2.Id };
        var userDto = await _handler.HandleAsync(query);

        Assert.NotNull(userDto);
        Assert.IsType<PublicUserDto>(userDto);
        Assert.Equal(user2.Id.Value, userDto.Id);
    }

    [Fact]
    public async Task given_users_exists_and_are_not_in_a_match_get_public_user_returns_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var query = new GetPublicUser() { RequestByUserId = user1.Id, RequestWhoUserId = user2.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_user_not_exists_get_public_user_returns_null()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var query = new GetPublicUser() { RequestByUserId = user.Id, RequestWhoUserId = Guid.NewGuid() };
        var userDto = await _handler.HandleAsync(query);
        Assert.Null(userDto);
    }

    [Fact]
    public async Task request_by_nonexisting_user_returns_UserNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var query = new GetPublicUser() { RequestByUserId = Guid.NewGuid(), RequestWhoUserId = user.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_authorization_serivce_fail_GetPublicUser_throws_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var query = new GetPublicUser() { RequestByUserId = user.Id, RequestWhoUserId = user.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly GetPublicUserHandler _handler;
    private readonly Mock<ISpatial> _mockedSpatial;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public GetPublicUserHanlderTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _mockedSpatial = new Mock<ISpatial>();
        _authService = new Mock<IDatingAppAuthorizationService>();
        _handler = new GetPublicUserHandler(_testDb.DbContext, _mockedSpatial.Object, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}