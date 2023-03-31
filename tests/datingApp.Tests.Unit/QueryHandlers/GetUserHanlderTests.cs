using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.QueryHandlers;

public class GetUserHanlderTests
{
    [Fact]
    public async Task query_existing_user_should_return_public_user_dto()
    {
        var query = new GetPublicUser();
        query.UserId = 1;
        var user = await _handler.HandleAsync(query);
        Assert.NotNull(user);
        Assert.IsType<PublicUserDto>(user);
    }

    [Fact]
    public async Task query_nonexisting_user_should_return_null()
    {
        var query = new GetPublicUser();
        query.UserId = 2;
        var user = await _handler.HandleAsync(query);
        Assert.Null(user);
    }

    // Arrange
    private readonly GetPublicUserHandler _handler;
    public GetUserHanlderTests()
    {
        var settings = new UserSettings(1, Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(1, "111111111", "bademail@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);
        _handler = new GetPublicUserHandler(mockUserRepository.Object);
    }
}