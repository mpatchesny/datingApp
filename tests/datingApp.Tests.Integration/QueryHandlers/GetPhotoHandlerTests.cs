using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Handlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetPhotoHandlerTests : IDisposable
{
    [Fact]
    public async void given_photo_exists_GetPhotoHandler_returns_proper_photo_Dto()
    {
        var photos1 = new List<Photo>() { IntegrationTestHelper.CreatePhoto(), IntegrationTestHelper.CreatePhoto(), IntegrationTestHelper.CreatePhoto(), };
        var photos2 = new List<Photo>() { IntegrationTestHelper.CreatePhoto(), IntegrationTestHelper.CreatePhoto(), IntegrationTestHelper.CreatePhoto(), };
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos1);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos2);
        _dbContext.ChangeTracker.Clear();
        
        var photoDto = await _handler.HandleAsync(new GetPhoto { PhotoId = photos1[1].Id });

        Assert.NotNull(photoDto);
        Assert.IsType<PhotoDto>(photoDto);
        Assert.Equal(photos1[1].Id.Value, photoDto.Id);
        Assert.Equal(user1.Id.Value, photoDto.UserId);
    }

    [Fact]
    public async void given_photo_not_exists_GetPhotoHandler_returns_PhotoNotExistsException()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(new GetPhoto { PhotoId = Guid.NewGuid() }));

        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly GetPhotoHandler _handler;
    public GetPhotoHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _handler = new GetPhotoHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}