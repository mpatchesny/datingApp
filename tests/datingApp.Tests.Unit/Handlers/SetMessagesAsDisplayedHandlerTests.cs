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
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Unit.Handlers;

public class SetMessagesAsDisplayedHandlerTests
{
    [Fact]
    public async Task given_match_or_message_not_exists_SendMessageHandler_throws_MessageNotExistsException()
    {
        
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByMessageIdAsync(It.IsAny<MessageId>(), It.IsAny<Expression<Func<Match, IEnumerable<Message>>>>()))
            .Returns(Task.FromResult<Match>(null));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new SetMessagesAsDisplayed(Guid.NewGuid(), Guid.NewGuid());
        var handler = new SetMessagesAsDisplayedHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<MessageNotExistsException>(exception);
    }

    [Fact]
    public async Task given_match_exists_and_authorization_failed_SendMessageHandler_throws_MatchNotExistsException()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByMessageIdAsync(It.IsAny<MessageId>(), It.IsAny<Expression<Func<Match, IEnumerable<Message>>>>()))
            .Returns(Task.FromResult<Match>(match));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var command = new SetMessagesAsDisplayed(Guid.NewGuid(), Guid.NewGuid());
        var handler = new SetMessagesAsDisplayedHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_match_exists_and_authorization_succeed_SendMessageHandler_should_succeed()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var messages = new List<Message>()
        {
            new Message(Guid.NewGuid(), userId1, "hello", false, DateTime.UtcNow),
            new Message(Guid.NewGuid(), userId1, "hello", false, DateTime.UtcNow.AddSeconds(-1)),
            new Message(Guid.NewGuid(), userId1, "hello", false, DateTime.UtcNow.AddSeconds(-2)),
            new Message(Guid.NewGuid(), userId1, "hello", false, DateTime.UtcNow.AddSeconds(-3))
        };
        var match = new Match(Guid.NewGuid(), userId1, userId2, DateTime.UtcNow, messages: messages);
        var repository = new Mock<IMatchRepository>();
        repository.Setup(x => x.GetByMessageIdAsync(It.IsAny<MessageId>(), It.IsAny<Expression<Func<Match, IEnumerable<Message>>>>()))
            .Returns(Task.FromResult<Match>(match));
        repository.Setup(x => x.UpdateAsync(It.IsAny<Match>()));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new SetMessagesAsDisplayed(messages[0].Id, match.UserId2);
        var handler = new SetMessagesAsDisplayedHandler(repository.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        repository.Verify(x => x.GetByMessageIdAsync(command.LastMessageId, It.IsAny<Expression<Func<Match, IEnumerable<Message>>>>()), Times.Once());
        repository.Verify(x => x.UpdateAsync(match), Times.Once());
        authorizationService.Verify(x => x.AuthorizeAsync(command.AuthenticatedUserId, match, "OwnerPolicy"), Times.Once());
        Assert.True(match.Messages.All(x => x.IsDisplayed == true));
    }
}