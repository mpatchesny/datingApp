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

public class GetMatchesHandlerTests
{
    [Fact]
    public async Task query_matches_by_existing_user_id_should_return_nonempty_collection_of_matches_dto()
    {
        var query = new GetMatches();
        query.UserId = 1;
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(1, matches.Count());
        Assert.IsType<MatchDto>(matches.First());
    }

    [Fact]
    public async Task query_matches_by_nonexisting_user_id_should_return_empty_collection()
    {
        var query = new GetMatches();
        query.UserId = 0;
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(0, matches.Count());
    }
    
    // Arrange
    private readonly GetMatchesHandler _handler;
    public GetMatchesHandlerTests()
    {
        var settings = new UserSettings(1, Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(1, "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var match = new Core.Entities.Match(1, 1, 1, null, DateTime.UtcNow);

        var mockMatchRepository = new Mock<IMatchRepository>();
        mockMatchRepository
            .Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(new List<Core.Entities.Match> { match } );
        _handler = new GetMatchesHandler(mockMatchRepository.Object);
    }
}