using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeleteMatchHandler : ICommandHandler<DeleteMatch>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IDeletedEntityService _deletedEntityService;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public DeleteMatchHandler(IMatchRepository matchRepository, IDeletedEntityService deletedEntityRepository, IDatingAppAuthorizationService authorizationService)
    {
        _matchRepository = matchRepository;
        _deletedEntityService = deletedEntityRepository;
        _authorizationService = authorizationService;
    }
    public async Task HandleAsync(DeleteMatch command)
    {
        var match = await _matchRepository.GetByIdAsync(command.MatchId);
        if (match == null)
        {
            if (await _deletedEntityService.ExistsAsync(command.MatchId))
            {
                throw new MatchAlreadyDeletedException(command.MatchId);
            }
            else
            {
                throw new MatchNotExistsException(command.MatchId);
            }
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, match, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        await _matchRepository.DeleteAsync(match);
        await _deletedEntityService.AddAsync(match.Id);
    }
}