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

        double xne = query.Lat + query.Range * 0.009;
        double yne = query.Lon + query.Range * 0.009;

        double xnw = query.Lat - query.Range * 0.009;
        double ynw = query.Lon + query.Range * 0.009;

        double xse = query.Lat + query.Range * 0.009;
        double yse = query.Lon - query.Range * 0.009;

        double xsw = query.Lat - query.Range * 0.009;
        double ysw = query.Lon - query.Range * 0.009;

        // we want only candidates who haven't been swiped by user who requests
        // and are different from user who requests candidates
        var swippedCandidates = 
            _dbContext.Swipes.Where(s => s.SwippedById == query.UserId).Select(x => x.SwippedWhoId);

        // we want candidates that matches sex, age and range of user who requests
        // we want candidates sorted by number of likes descending
        var candidates = _dbContext.Users
                    .Where(x => x.Id != query.UserId)
                    .Where(x => !swippedCandidates.Contains(x.Id))
                    .Where(x => ((int) x.Sex & query.Sex) > 0)
                    .Where(x => x.Settings.Lat <= yne)
                    .Where(x => x.Settings.Lat <= ynw)
                    .Where(x => x.Settings.Lat >= ysw)
                    .Where(x => x.Settings.Lat >= yse)
                    .Where(x => x.Settings.Lon <= xne)
                    .Where(x => x.Settings.Lon <= xse)
                    .Where(x => x.Settings.Lon >= xnw)
                    .Where(x => x.Settings.Lon >= xsw)
                    .Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob)
                    .Include(x => x.Photos)
                    .AsNoTracking();

        return candidates.Select(x => x.AsPublicDto()).ToList();
    }
}