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
[Authorize]
[Route("matches")]
public class MatchesController : ApiControllerBase
{
    private readonly IQueryHandler<GetMatches, PaginatedDataDto> _getMatchesHandler;
    private readonly IQueryHandler<GetMatch, MatchDto> _getMatchHandler;
    private readonly IQueryHandler<GetMessages, PaginatedDataDto> _getMessagesHandler;
    private readonly IQueryHandler<GetMessage, MessageDto> _getMessageHandler;
    private readonly ICommandHandler<SendMessage> _sendMessageHandler;
    private readonly ICommandHandler<SetMessagesAsDisplayed> _setMessagesAsDisplayedHandler;
    private readonly ICommandHandler<SetMatchAsDisplayed> _setMatchAsDisplayedHandler;
    private readonly ICommandHandler<DeleteMatch> _deleteMatchHandler;
    public MatchesController(IQueryHandler<GetMatches, PaginatedDataDto> getMatchesHandler,
                            ICommandHandler<SendMessage> sendMessageHandler,
                            ICommandHandler<DeleteMatch> deleteMatchHandler,
                            IQueryHandler<GetMessages, PaginatedDataDto> getMessagesHandler,
                            IQueryHandler<GetMessage, MessageDto> getMessageHandler,
                            ICommandHandler<SetMessagesAsDisplayed> setMessagesAsDisplayedHandler,
                            ICommandHandler<SetMatchAsDisplayed> setMatchAsDisplayedHandler,
                            IQueryHandler<GetMatch, MatchDto> getMatchHandler)
    {
        _getMatchesHandler = getMatchesHandler;
        _deleteMatchHandler = deleteMatchHandler;
        _getMessagesHandler = getMessagesHandler;
        _sendMessageHandler = sendMessageHandler;
        _getMessageHandler = getMessageHandler;
        _setMessagesAsDisplayedHandler = setMessagesAsDisplayedHandler;
        _setMatchAsDisplayedHandler = setMatchAsDisplayedHandler;
        _getMatchHandler = getMatchHandler;
    }

    [HttpGet("{matchId:guid}")]
    public async Task<ActionResult<MatchDto>> GetMatch(Guid matchId)
    {
        var query = new GetMatch { MatchId = matchId };
        query = Authenticate(query);
        query.UserId = AuthenticatedUserId;
        return Ok(await _getMatchHandler.HandleAsync(query));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedDataDto>> GetMatches([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var query = Authenticate(new GetMatches { UserId = AuthenticatedUserId });
        query.SetPage(page);
        query.SetPageSize(pageSize);
        return Ok(await _getMatchesHandler.HandleAsync(query));
    }

    [HttpGet("{matchId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> GetMessage(Guid matchId, Guid messageId)
    {
        var query = Authenticate(new GetMessage { MessageId = messageId });
        var message = await _getMessageHandler.HandleAsync(query);
        if (message == null)
        {
            return NotFound();
        }
        return message;
    }

    [HttpGet("{matchId:guid}/messages")]
    public async Task<ActionResult<PaginatedDataDto>> GetMessages(Guid matchId, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var query = Authenticate(new GetMessages { MatchId = matchId });
        query.SetPage(page);
        query.SetPageSize(pageSize);
        return Ok(await _getMessagesHandler.HandleAsync(query));
    }

    [HttpPost("{matchId:guid}/messages")]
    public async Task<ActionResult> SendMessage([FromRoute] Guid matchId, [FromBody] SendMessage command)
    {
        command = Authenticate(command);
        command = command with {MessageId = Guid.NewGuid(), MatchId = matchId};
        await _sendMessageHandler.HandleAsync(command);

        var query = Authenticate(new GetMessage { MessageId = command.MessageId });
        var message = await _getMessageHandler.HandleAsync(query);
        return CreatedAtAction(nameof(GetMessage), new { command.MatchId, command.MessageId }, message);
    }

    [HttpPatch("{matchId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult> ChangeMessage([FromRoute] Guid matchId, [FromRoute] Guid messageId, [FromBody] SetMessagesAsDisplayed command)
    {
        command = Authenticate(command);
        command = command with {LastMessageId = messageId};
        await _setMessagesAsDisplayedHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPatch("{matchId:guid}")]
    public async Task<ActionResult> ChangeMatch([FromRoute] Guid matchId, [FromBody] SetMatchAsDisplayed command)
    {
        command = Authenticate(command);
        command = command with {MatchId = matchId};
        await _setMatchAsDisplayedHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{matchId:guid}")]
    public async Task<ActionResult> Delete(Guid matchId)
    {
        var command = Authenticate(new DeleteMatch(matchId));
        await _deleteMatchHandler.HandleAsync(command);
        return NoContent();
    }
}