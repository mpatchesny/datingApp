using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
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
        var photo = await _handler.HandleAsync(new GetPhoto { PhotoId = 1 });
        Assert.NotNull(photo);
        Assert.IsType<PhotoDto>(photo);
    }

    [Fact]
    public async void get_nonexisting_photo_should_return_null()
    {
        var photo = await _handler.HandleAsync(new GetPhoto { PhotoId = 2 });
        Assert.Null(photo);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetPhotoHandler _handler;
    public GetPhotoHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        
        var photos = new List<Photo>{
            new Photo(0, Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 1)
        };
        
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Photos.AddRange(photos);
        _testDb.DbContext.SaveChanges();
        _handler = new GetPhotoHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}