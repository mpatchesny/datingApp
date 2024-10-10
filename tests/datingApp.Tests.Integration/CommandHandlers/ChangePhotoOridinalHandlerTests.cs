using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Entities;
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
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Photo>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var command = new ChangePhotoOridinal(photo.Id, 2);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_photo_exists_and_has_that_oridinal_change_oridinal_should_succeed()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Photo>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo1 = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);
        var photo2 = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, 2);

        var command = new ChangePhotoOridinal(photo1.Id, 2);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_photo_not_exists_change_oridinal_throws_PhotoNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Photo>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangePhotoOridinal(Guid.NewGuid(), 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }

    [Fact]
    public async Task given_authorization_fail_and_photo_exists_change_oridinal_throws_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Photo>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var command = new ChangePhotoOridinal(photo.Id, 2);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }
        
    // Arrange
    private readonly TestDatabase _testDb;
    private readonly ChangePhotoOridinalHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public ChangePhotoOridinalHandlerTests()
    {
        _testDb = new TestDatabase();
        var mockedPhotoOrderer = new Mock<IPhotoOrderer>();
        mockedPhotoOrderer.Setup(m => m.OrderPhotos(It.IsAny<List<Photo>>(), It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns((List<Photo> x, Guid y, int z) => x);
        var photoRepository = new DbPhotoRepository(_testDb.DbContext);
        _authService = new Mock<IDatingAppAuthorizationService>();
        _handler = new ChangePhotoOridinalHandler(photoRepository, mockedPhotoOrderer.Object, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}