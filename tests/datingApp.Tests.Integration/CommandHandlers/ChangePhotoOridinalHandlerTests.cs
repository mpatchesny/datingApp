using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class ChangePhotoOridinalHandlerTests : IDisposable
{
    [Fact]
    public async Task given_photo_exists_change_oridinal_should_succeed()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto(), };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var command = new ChangePhotoOridinal(photos[0].Id, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_photo_exists_and_has_that_oridinal_change_oridinal_should_succeed()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto(0), IntegrationTestHelper.CreatePhoto(1), };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var command = new ChangePhotoOridinal(photos[0].Id, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_photo_not_exists_change_oridinal_throws_PhotoNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangePhotoOridinal(Guid.NewGuid(), 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }

    [Fact]
    public async Task given_authorization_fail_and_photo_exists_change_oridinal_throws_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto(0),  };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var command = new ChangePhotoOridinal(photos[0].Id, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }
        
    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly ChangePhotoOridinalHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public ChangePhotoOridinalHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        var userRepository = new DbUserRepository(_dbContext);
        _authService = new Mock<IDatingAppAuthorizationService>();
        _handler = new ChangePhotoOridinalHandler(userRepository, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}