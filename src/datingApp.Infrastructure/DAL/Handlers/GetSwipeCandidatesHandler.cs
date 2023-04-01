using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetSwipeCandidatesHandler : IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>>
{
    private readonly DatingAppDbContext _dbContext;
    public GetSwipeCandidatesHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PublicUserDto>> HandleAsync(GetSwipeCandidates query)
    {
        // Chcemy użytkowników, którzy:
        // - TODO są w odpowiedniej odległości
        // FIXME: async
        var candidatesQuery = from u in _dbContext.Users
            from s in _dbContext.Swipes.Where(s => s.SwippedById != u.Id).DefaultIfEmpty()
            select new { u, s };
        var candidates = candidatesQuery
                        .Where(x => x.u.GetAge() >= query.AgeFrom && x.u.GetAge() <= query.AgeTo)
                        .Where(x => (int) x.u.Sex == query.Sex)
                        .Include(x => x.u.Photos)
                        .AsNoTracking();
        return candidates.Select(x => x.u.AsPublicDto()).ToList();
    }
}