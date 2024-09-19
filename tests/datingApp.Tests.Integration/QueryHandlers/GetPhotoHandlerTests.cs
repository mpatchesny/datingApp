using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetPhotoHandlerTests : IDisposable
{
    [Fact]
    public async void get_existing_photo_should_return_photo_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var photoDto = await _handler.HandleAsync(new GetPhoto { PhotoId = photo.Id });
        Assert.NotNull(photoDto);
        Assert.IsType<PhotoDto>(photoDto);
    }

    [Fact]
    public async void get_nonexisting_photo_should_return_photo_not_exists_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(new GetPhoto { PhotoId = Guid.NewGuid() }));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetPhotoHandler _handler;
    public GetPhotoHandlerTests()
    {
        _testDb = new TestDatabase();
        _handler = new GetPhotoHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}