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
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryHandler<GetMatches, PaginatedDataDto> _getMatchesHandler;
    private readonly IQueryHandler<GetMatch, MatchDto> _getMatchHandler;
    private readonly IQueryHandler<GetMessages, PaginatedDataDto> _getMessagesHandler;
    private readonly IQueryHandler<GetMessage, MessageDto> _getMessageHandler;
    public MatchesController(ICommandDispatcher commandDispatcher,
                            IQueryHandler<GetMatches, PaginatedDataDto> getMatchesHandler,
                            IQueryHandler<GetMessages, PaginatedDataDto> getMessagesHandler,
                            IQueryHandler<GetMessage, MessageDto> getMessageHandler,
                            IQueryHandler<GetMatch, MatchDto> getMatchHandler)
    {
        _commandDispatcher = commandDispatcher;
        _getMatchesHandler = getMatchesHandler;
        _getMessagesHandler = getMessagesHandler;
        _getMessageHandler = getMessageHandler;
        _getMatchHandler = getMatchHandler;
    }

    [HttpGet("{matchId:guid}")]
    public async Task<ActionResult<MatchDto>> GetMatch(Guid matchId)
    {
        var query = new GetMatch { MatchId = matchId };
        query = Authenticate(query);
        query.UserId = AuthenticatedUserId;
        var match = await _getMatchHandler.HandleAsync(query);
        return Ok(match);
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
        command = command with {MessageId = Guid.NewGuid(), SendFromId = command.AuthenticatedUserId, MatchId = matchId};
        await _commandDispatcher.DispatchAsync(command);

        var query = Authenticate(new GetMessage { MessageId = command.MessageId });
        var message = await _getMessageHandler.HandleAsync(query);
        return CreatedAtAction(nameof(GetMessage), new { command.MatchId, command.MessageId }, message);
    }

    [HttpPatch("{matchId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult> ChangeMessage([FromRoute] Guid matchId, [FromRoute] Guid messageId, [FromBody] SetMessagesAsDisplayed command)
    {
        command = Authenticate(command);
        command = command with {LastMessageId = messageId};
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }

    [HttpPatch("{matchId:guid}")]
    public async Task<ActionResult> ChangeMatch([FromRoute] Guid matchId, [FromBody] SetMatchAsDisplayed command)
    {
        command = Authenticate(command);
        command = command with {MatchId = matchId};
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }

    [HttpDelete("{matchId:guid}")]
    public async Task<ActionResult> Delete(Guid matchId)
    {
        var command = Authenticate(new DeleteMatch(matchId));
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }
}