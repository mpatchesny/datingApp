using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Infrastructure.Spatial;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetSwipeCandidatesHandler : IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;
    public GetSwipeCandidatesHandler(DatingAppDbContext dbContext, ISpatial spatial)
    {
        _dbContext = dbContext;
        _spatial = spatial;
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

        var square = _spatial.GetApproxSquareAroundPoint(query.Lat, query.Lon, query.Range);

        // we want only candidates who haven't been swiped by user who requests
        // and are different from user who requests candidates
        var swippedCandidates = 
            _dbContext.Swipes.Where(s => s.SwippedById == query.UserId).Select(x => x.SwippedWhoId);

        // we want candidates that matches sex, age and range of user who requests
        // we want only a number of candidates as in query
        // we want candidates sorted by number of likes descending
        var candidates = _dbContext.Users
                    .Where(x => x.Id != query.UserId)
                    .Where(x => !swippedCandidates.Contains(x.Id))
                    .Where(x => ((int) x.Sex & query.Sex) > 0)
                    .Where(x => x.Settings.Lat <= square[0].lat)
                    .Where(x => x.Settings.Lat <= square[1].lat)
                    .Where(x => x.Settings.Lat >= square[2].lat)
                    .Where(x => x.Settings.Lat >= square[3].lat)
                    .Where(x => x.Settings.Lon <= square[0].lon)
                    .Where(x => x.Settings.Lon <= square[1].lon)
                    .Where(x => x.Settings.Lon >= square[2].lon)
                    .Where(x => x.Settings.Lon >= square[3].lon)
                    .Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob)
                    .Take(query.HowMany)
                    .Include(x => x.Photos)
                    .AsNoTracking()
                    .ToList();

        var candidatesList = candidates
                            .Select(u => u.AsPublicDto(_spatial.CalculateDistance(query.Lat, query.Lon, u.Settings.Lat, u.Settings.Lon)))
                            .ToList();

        return candidatesList;
    }
}