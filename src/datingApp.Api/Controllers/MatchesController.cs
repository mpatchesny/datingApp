using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Route("matches")]
public class MatchesController : ControllerBase
{
    private readonly IQueryHandler<GetMatches, IEnumerable<MatchDto>> _getMatchesHandler;
    private readonly IQueryHandler<GetMessages, IEnumerable<MessageDto>> _getMessagesHandler;
    private readonly ICommandHandler<SendMessage> _sendMessageHandler;
    private readonly ICommandHandler<DeleteMatch> _deleteMatchHandler;
    public MatchesController(IQueryHandler<GetMatches, IEnumerable<MatchDto>> getMatchesHandler,
                            ICommandHandler<SendMessage> sendMessageHandler,
                            ICommandHandler<DeleteMatch> deleteMatchHandler,
                            IQueryHandler<GetMessages, IEnumerable<MessageDto>> getMessagesHandler)
    {
        _getMatchesHandler = getMatchesHandler;
        _deleteMatchHandler = deleteMatchHandler;
        _getMessagesHandler = getMessagesHandler;
        _sendMessageHandler = sendMessageHandler;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> Get(Guid userId)
    {
        return Ok(await _getMatchesHandler.HandleAsync(new GetMatches { UserId = userId }));
    }

    [HttpGet("{matchId:guid}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(Guid matchId)
    {
        return Ok(await _getMessagesHandler.HandleAsync(new GetMessages { MatchId = matchId }));
    }

    [HttpPost("{matchId:guid}/messages")]
    public async Task<ActionResult> SendMessage(SendMessage command)
    {
        command = command with {MessageId = Guid.NewGuid()};
        await _sendMessageHandler.HandleAsync(command);
        // FIXME:
        // return CreatedAtAction(nameof(GetPrivateUser), new { command.UserId });
        return Ok();
    }

    [HttpDelete("{matchId:guid}")]
    public async Task<ActionResult> Delete(Guid matchId)
    {
        await _deleteMatchHandler.HandleAsync(new DeleteMatch(matchId));
        return Ok();
    }

}