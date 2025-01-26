using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Unit.Handlers;

public class SetMatchAsDisplayedHandlerTests
{
    [Fact]
    public async Task given_match_not_exists_SetMatchAsDisplayed_throws_MatchNotExistsException()
    {
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>(), It.IsAny<Expression<Func<Match, IEnumerable<Message>>>>()))
            .Returns(Task.FromResult<Match>(null));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new SetMatchAsDisplayed(Guid.NewGuid(), Guid.NewGuid());
        var handler = new SetMatchAsDisplayedHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    [Fact]
    public async Task given_match_exists_and_authorization_failed_SetMatchAsDisplayed_throws_MatchNotExistsException()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>(), It.IsAny<Expression<Func<Match, IEnumerable<Message>>>>()))
            .Returns(Task.FromResult<Match>(match));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var command = new SetMatchAsDisplayed(match.Id, match.UserId1);
        var handler = new SetMatchAsDisplayedHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_match_exists_and_authorization_succeed_SetMatchAsDisplayed_should_succeed()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>(), It.IsAny<Expression<Func<Match, IEnumerable<Message>>>>()))
            .Returns(Task.FromResult<Match>(match));
        repository.Setup(x => x.UpdateAsync(It.IsAny<Match>()));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new SetMatchAsDisplayed(match.Id, match.UserId1);
        var handler = new SetMatchAsDisplayedHandler(repository.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        repository.Verify(x => x.GetByIdAsync(command.MatchId, It.IsAny<Expression<Func<Match, IEnumerable<Message>>>>()), Times.Once());
        repository.Verify(x => x.UpdateAsync(match), Times.Once());
        authorizationService.Verify(x => x.AuthorizeAsync(command.AuthenticatedUserId, match, "OwnerPolicy"), Times.Once());
        Assert.True(match.IsDisplayedByUser(match.UserId1));
    }
}