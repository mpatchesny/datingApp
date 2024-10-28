using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.DAL.Repositories;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class AddMatchHandlerTests
{
    [Fact]
    public async Task add_match_should_invoke_repository_add_match_once_when_match_not_exists()
    {
        var command = new AddMatch(Guid.NewGuid(), Guid.NewGuid());
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(m => m.ExistsAsync(It.IsAny<UserId>(), It.IsAny<UserId>())).ReturnsAsync(true);

        var handler = new AddMatchHandler(matchRepository.Object);
        await handler.HandleAsync(command);
        matchRepository.Verify(mock => mock.AddAsync(It.IsAny<Core.Entities.Match>()), Times.Once());
    }

    [Fact]
    public async Task add_match_should_not_invoke_repository_add_match_when_match_exists()
    {
        var command = new AddMatch(Guid.NewGuid(), Guid.NewGuid());
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(m => m.ExistsAsync(It.IsAny<UserId>(), It.IsAny<UserId>())).ReturnsAsync(true);

        var handler = new AddMatchHandler(matchRepository.Object);
        await handler.HandleAsync(command);
        matchRepository.Verify(mock => mock.AddAsync(It.IsAny<Core.Entities.Match>()), Times.Never());
    }
}