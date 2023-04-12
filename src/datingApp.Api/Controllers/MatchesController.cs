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
    private readonly IQueryHandler<GetMessage, MessageDto> _getMessageHandler;
    private readonly ICommandHandler<SendMessage> _sendMessageHandler;
    private readonly ICommandHandler<ChangeMessage> _changeMessageHandler;
    private readonly ICommandHandler<DeleteMatch> _deleteMatchHandler;
    public MatchesController(IQueryHandler<GetMatches, IEnumerable<MatchDto>> getMatchesHandler,
                            ICommandHandler<SendMessage> sendMessageHandler,
                            ICommandHandler<DeleteMatch> deleteMatchHandler,
                            IQueryHandler<GetMessages, IEnumerable<MessageDto>> getMessagesHandler,
                            IQueryHandler<GetMessage, MessageDto> getMessageHandler,
                            ICommandHandler<ChangeMessage> changeMessageHandler)
    {
        _getMatchesHandler = getMatchesHandler;
        _deleteMatchHandler = deleteMatchHandler;
        _getMessagesHandler = getMessagesHandler;
        _sendMessageHandler = sendMessageHandler;
        _getMessageHandler = getMessageHandler;
        _changeMessageHandler = changeMessageHandler;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> Get(Guid userId)
    {
        return Ok(await _getMatchesHandler.HandleAsync(new GetMatches { UserId = userId }));
    }

    [HttpGet("{matchId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessage(Guid matchId, Guid messageId)
    {
        return Ok(await _getMessageHandler.HandleAsync(new GetMessage { MessageId = messageId }));
    }

    [HttpGet("{matchId:guid}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(Guid matchId)
    {
        return Ok(await _getMessagesHandler.HandleAsync(new GetMessages { MatchId = matchId }));
    }

    [HttpPost("{matchId:guid}/messages")]
    public async Task<ActionResult> SendMessage([FromRoute] Guid matchId, [FromBody] SendMessage command)
    {
        command = command with {MessageId = Guid.NewGuid()};
        command = command with {MatchId = matchId};
        await _sendMessageHandler.HandleAsync(command);
        var message = await _getMessageHandler.HandleAsync(new GetMessage { MessageId = command.MessageId });
        return CreatedAtAction(nameof(GetMessage), new { command.MatchId, command.MessageId }, message);
    }

    [HttpPatch("{matchId:guid}/messages")]
    public async Task<ActionResult> ChangeMessage([FromRoute] Guid matchId, [FromBody] ChangeMessage command)
    {
        await _changeMessageHandler.HandleAsync(command);
        var message = await _getMessageHandler.HandleAsync(new GetMessage { MessageId = command.MessageId });
        return CreatedAtAction(nameof(GetMessage), new { message.MatchId, command.MessageId }, message);
    }

    [HttpDelete("{matchId:guid}")]
    public async Task<ActionResult> Delete(Guid matchId)
    {
        await _deleteMatchHandler.HandleAsync(new DeleteMatch(matchId));
        return NoContent();
    }

}