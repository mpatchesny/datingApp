using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[ApiController]
[Route("matches")]
public class MatchesController : ApiControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    public MatchesController(ICommandDispatcher commandDispatcher,
                            IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{matchId:guid}")]
    public async Task<ActionResult<MatchDto>> GetMatch(Guid matchId)
    {
        var query = Authenticate(new GetMatch { MatchId = matchId, UserId = AuthenticatedUserId });
        var match = await _queryDispatcher.DispatchAsync<GetMatch, MatchDto>(query);
        return Ok(match);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedDataDto<MatchDto>>> GetMatches([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var query = Authenticate(new GetMatches { UserId = AuthenticatedUserId });
        query.SetPage(page);
        query.SetPageSize(pageSize);
        var result = await _queryDispatcher.DispatchAsync<GetMatches, PaginatedDataDto<MatchDto>>(query);
        return Ok(result);
    }

    [HttpGet("{matchId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> GetMessage(Guid matchId, Guid messageId)
    {
        var query = Authenticate(new GetMessage { MessageId = messageId });
        var message = await _queryDispatcher.DispatchAsync<GetMessage, MessageDto>(query);
        return message;
    }

    [HttpGet("{matchId:guid}/messages")]
    public async Task<ActionResult<PaginatedDataDto<MessageDto>>> GetMessages(Guid matchId, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var query = Authenticate(new GetMessages { MatchId = matchId });
        query.SetPage(page);
        query.SetPageSize(pageSize);
        var result = await _queryDispatcher.DispatchAsync<GetMessages, PaginatedDataDto<MessageDto>>(query);
        return Ok(result);
    }

    [HttpPost("{matchId:guid}/messages")]
    public async Task<ActionResult> SendMessage([FromRoute] Guid matchId, [FromBody] SendMessage command)
    {
        command = Authenticate(command);
        command = command with {MessageId = Guid.NewGuid(), SendFromId = command.AuthenticatedUserId, MatchId = matchId};
        await _commandDispatcher.DispatchAsync(command);

        var query = Authenticate(new GetMessage { MessageId = command.MessageId });
        var message = await _queryDispatcher.DispatchAsync<GetMessage, MessageDto>(query);
        return CreatedAtAction(nameof(GetMessage), new { command.MatchId, command.MessageId }, message);
    }

    [HttpPatch("{matchId:guid}")]
    public async Task<ActionResult> ChangeMatch([FromRoute] Guid matchId, [FromBody] SetMatchAsDisplayed command)
    {
        command = Authenticate(command with {MatchId = matchId});
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