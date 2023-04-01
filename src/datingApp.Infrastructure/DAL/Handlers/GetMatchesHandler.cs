using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchesHandler : IQueryHandler<GetMatches, IEnumerable<MatchDto>>
{
    private readonly IMatchRepository _matchRepository;
    public GetMatchesHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }
    
    public async Task<IEnumerable<MatchDto>> HandleAsync(GetMatches query)
    {
        var matches = await _matchRepository.GetByUserIdAsync(query.UserId);
        return matches.Select(x => x.AsDto()).ToList();
    }
}