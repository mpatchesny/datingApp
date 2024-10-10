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