using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Xunit.Sdk;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Unit.Handlers;

public class DeleteMatchHandlerTests
{
    [Fact]
    public async Task given_match_not_exists_and_match_id_not_in_deleted_entities_DeleteMatchHandler_returns_MatchNotExistsException()
    {
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>(), null))
            .Returns(Task.FromResult<Match>(null));

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeleteMatch(Guid.NewGuid());
        var handler = new DeleteMatchHandler(repository.Object, deletedEntityService.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    [Fact]
    public async Task given_match_not_exists_and_id_is_in_deleted_entities_DeleteMatchHandler_returns_MatchAlreadyDeletedException()
    {
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>(), null))
            .Returns(Task.FromResult<Core.Entities.Match>(null));

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.AddAsync(It.IsAny<Guid>(), "match", DateTime.UtcNow));
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(true));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeleteMatch(Guid.NewGuid());
        var handler = new DeleteMatchHandler(repository.Object, deletedEntityService.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<MatchAlreadyDeletedException>(exception);
    }
    
    [Fact]
    public async Task given_authorization_fail_DeleteMatchHandler_returns_UnauthorizedException()
    {
        var match = new Core.Entities.Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>(), null))
            .Returns(Task.FromResult<Core.Entities.Match>(match));

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.AddAsync(It.IsAny<Guid>(), "match", DateTime.UtcNow));
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var command = new DeleteMatch(Guid.NewGuid());
        var handler = new DeleteMatchHandler(repository.Object, deletedEntityService.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_match_exists_and_authorization_succeed_and_match_id_not_in_deleted_entities_DeleteMatchHandler_succeed()
    {
        var match = new Core.Entities.Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>(), null))
            .Returns(Task.FromResult<Core.Entities.Match>(match));
        repository.Setup(x => x.DeleteAsync(It.IsAny<Match>()));

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.AddAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()));
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeleteMatch(Guid.NewGuid());
        var handler = new DeleteMatchHandler(repository.Object, deletedEntityService.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        authorizationService.Verify(x => x.AuthorizeAsync(command.AuthenticatedUserId, match, "OwnerPolicy"), Times.Once());
        repository.Verify(x => x.GetByIdAsync(command.MatchId, null), Times.Once());
        repository.Verify(x => x.DeleteAsync(match), Times.Once());
        deletedEntityService.Verify(x => x.ExistsAsync(match.Id), Times.Never());
        deletedEntityService.Verify(x => x.AddAsync(match.Id, "match", It.IsAny<DateTime>()), Times.Once());
    }
}