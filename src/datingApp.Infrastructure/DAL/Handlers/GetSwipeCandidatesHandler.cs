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
        // FIXME: async
        // Chcemy użytkowników, którzy:
        // - są w odpowiedniej odległości
        // - sortowanie malejąco wg popularności (liczby like)

        var now = DateTime.UtcNow;
        DateOnly minDob = new DateOnly(now.Year - query.AgeTo, now.Month, now.Day);
        DateOnly maxDob = new DateOnly(now.Year - query.AgeFrom, now.Month, now.Day);

        // we want only candidates who haven't been swiped
        // and are different from user who requests candidates
        var notSwippedCandidates = 
            from u in _dbContext.Users.Where(x => x.Id != query.UserId)
            from s in _dbContext.Swipes.Where(s => s.SwippedById != query.UserId).DefaultIfEmpty()
            select new { u.Id };

        // we want candidates who matches sex, age and range
        // we want candidates sorted by number of likes descending
        var candidates = _dbContext.Users
                        .Where(x => notSwippedCandidates.Select(u => u.Id).Contains(x.Id))
                        .Where(x => ((int) x.Sex & query.Sex) != 0)
                        .Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob)
                        .Include(x => x.Photos)
                        .AsNoTracking();

        return candidates.Select(x => x.AsPublicDto()).ToList();
    }
}