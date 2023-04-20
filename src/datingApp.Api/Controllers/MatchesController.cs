using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ICommandHandler<SetMessageAsDisplayed> _setMessageAsDisplayedHandler;
    private readonly ICommandHandler<SetMatchAsDisplayed> _setMatchAsDisplayedHandler;
    private readonly ICommandHandler<DeleteMatch> _deleteMatchHandler;
    public MatchesController(IQueryHandler<GetMatches, IEnumerable<MatchDto>> getMatchesHandler,
                            ICommandHandler<SendMessage> sendMessageHandler,
                            ICommandHandler<DeleteMatch> deleteMatchHandler,
                            IQueryHandler<GetMessages, IEnumerable<MessageDto>> getMessagesHandler,
                            IQueryHandler<GetMessage, MessageDto> getMessageHandler,
                            ICommandHandler<SetMessageAsDisplayed> setMessageAsDisplayedHandler,
                            ICommandHandler<SetMatchAsDisplayed> setMatchAsDisplayedHandler)
    {
        _getMatchesHandler = getMatchesHandler;
        _deleteMatchHandler = deleteMatchHandler;
        _getMessagesHandler = getMessagesHandler;
        _sendMessageHandler = sendMessageHandler;
        _getMessageHandler = getMessageHandler;
        _setMessageAsDisplayedHandler = setMessageAsDisplayedHandler;
        _setMatchAsDisplayedHandler = setMatchAsDisplayedHandler;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatchDto>>> Get([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var userId = Guid.Parse(User.Identity?.Name);
        var command = new GetMatches { UserId = userId };
        command.SetPage(page);
        command.SetPageSize(pageSize);
        return Ok(await _getMatchesHandler.HandleAsync(command));
    }

    [Authorize]
    [HttpGet("{matchId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> GetMessage(Guid matchId, Guid messageId)
    {
        var message = await _getMessageHandler.HandleAsync(new GetMessage { MessageId = messageId });
        if (message == null)
        {
            return NotFound();
        }
        return message;
    }

    [Authorize]
    [HttpGet("{matchId:guid}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(Guid matchId, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var command = new GetMessages { MatchId = matchId };
        command.SetPage(page);
        command.SetPageSize(pageSize);
        return Ok(await _getMessagesHandler.HandleAsync(command));
    }

    [Authorize]
    [HttpPost("{matchId:guid}/messages")]
    public async Task<ActionResult> SendMessage([FromRoute] Guid matchId, [FromBody] SendMessage command)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var userId = Guid.Parse(User.Identity?.Name);
        command = command with {MessageId = Guid.NewGuid()};
        command = command with {MatchId = matchId};
        await _sendMessageHandler.HandleAsync(command);
        var message = await _getMessageHandler.HandleAsync(new GetMessage { MessageId = command.MessageId });
        return CreatedAtAction(nameof(GetMessage), new { command.MatchId, command.MessageId }, message);
    }

    [Authorize]
    [HttpPatch("{matchId:guid}/messages")]
    public async Task<ActionResult> ChangeMessage([FromRoute] Guid matchId, [FromBody] SetMessageAsDisplayed command)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var userId = Guid.Parse(User.Identity?.Name);
        command = command with {DisplayedByUserId = userId};
        await _setMessageAsDisplayedHandler.HandleAsync(command);
        return NoContent();
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult> ChangeMatch(SetMatchAsDisplayed command)
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name)) return NotFound();
        var userId = Guid.Parse(User.Identity?.Name);
        command = command with {UserId = userId};
        await _setMatchAsDisplayedHandler.HandleAsync(command);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{matchId:guid}")]
    public async Task<ActionResult> Delete(Guid matchId)
    {
        await _deleteMatchHandler.HandleAsync(new DeleteMatch(matchId));
        return NoContent();
    }

}