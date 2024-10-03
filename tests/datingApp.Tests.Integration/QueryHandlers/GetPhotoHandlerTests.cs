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
    public async void given_photo_exists_get_photo_returns_photo_dto_with_url_with_proper_extension()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var photoDto = await _handler.HandleAsync(new GetPhoto { PhotoId = photo.Id });
        Assert.NotNull(photoDto);
        Assert.IsType<PhotoDto>(photoDto);
        Assert.Equal("abc.bmp", photoDto.Url);
    }

    [Fact]
    public async void given_photo_not_exists_get_photo_returns_PhotoNotExistsException()
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