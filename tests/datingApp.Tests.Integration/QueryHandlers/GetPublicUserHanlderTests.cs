using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.Spatial;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;

public class GetPublicUserHanlderTests : IDisposable
{
    [Fact]
    public async Task given_authorization_serivce_success_query_existing_user_should_return_public_user_dto()
    {
        _mockedAuthService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);

        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var query = new GetPublicUser() { UserId = user.Id, UserRequestedId = user.Id };
        var userDto = await _handler.HandleAsync(query);

        Assert.NotNull(userDto);
        Assert.IsType<PublicUserDto>(userDto);
    }

    [Fact]
    public async Task query_nonexisting_user_should_user_not_exists_exception()
    {
        _mockedAuthService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);

        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var query = new GetPublicUser() { UserId = Guid.NewGuid(), UserRequestedId = user.Id };
        var userDto = await _handler.HandleAsync(query);

        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task request_by_nonexisting_user_should_return_null()
    {
        _mockedAuthService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);

        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var query = new GetPublicUser() { UserId = user.Id, UserRequestedId = Guid.NewGuid() };

        var userDto = await _handler.HandleAsync(query);
        Assert.Null(userDto);
    }

    [Fact]
    public async Task given_authorization_serivce_fail_GetPublicUser_throws_UnauthorizedException()
    {
        _mockedAuthService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Core.Entities.Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(0.0, 0.0, 0.0, 0.0)).Returns(0);

        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var query = new GetPublicUser() { UserId = user.Id, UserRequestedId = Guid.NewGuid() };

        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetPublicUserHandler _handler;
    private readonly Mock<ISpatial> _mockedSpatial;
    private readonly Mock<IDatingAppAuthorizationService> _mockedAuthService;
    public GetPublicUserHanlderTests()
    {
        _testDb = new TestDatabase();
        _mockedSpatial = new Mock<ISpatial>();
        _mockedAuthService = new Mock<IDatingAppAuthorizationService>();
        _handler = new GetPublicUserHandler(_testDb.DbContext, _mockedSpatial.Object, _mockedAuthService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}