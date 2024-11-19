using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
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

    private async Task<List<Guid>> GetInitialCandidatesToSwipe(UserSettings settings, Guid requestedById)
    {
        // we want only candidates who haven't been swiped by user who requested
        // we want candidates that matches sex, age and are within approx range of user who requested
        // we want candidates that are different from user who requested
        var now = DateTime.UtcNow;
        var minDob = new DateOnly(now.Year - settings.PreferredAge.To, now.Month, now.Day);
        var maxDob = new DateOnly(now.Year - settings.PreferredAge.From, now.Month, now.Day);

        int preferredSex = (int) settings.PreferredSex;

        var spatialSquare = _spatial.GetApproxSquareAroundPoint(settings.Location.Lat, settings.Location.Lon, 
            settings.PreferredMaxDistance + 5);

        var swipedCandidates = await _dbContext.Swipes
            .Where(swipe => swipe.SwipedById.Equals(requestedById))
            .Select(swipe => swipe.SwipedWhoId.Value)
            .ToListAsync();

        return await _dbContext.Users
            .Where(candidate => !candidate.Id.Equals(requestedById))
            .Where(candidate => !swipedCandidates.Contains(candidate.Id))
            .Where(candidate => ((int) candidate.Sex & preferredSex) != 0)
            .Where(candidate => candidate.DateOfBirth >= minDob)
            .Where(candidate => candidate.DateOfBirth <= maxDob)
            .Where(candidate => candidate.Settings.Location.Lat <= spatialSquare.NorthLat)
            .Where(candidate => candidate.Settings.Location.Lat >= spatialSquare.SouthLat)
            .Where(candidate => candidate.Settings.Location.Lon <= spatialSquare.EastLon)
            .Where(candidate => candidate.Settings.Location.Lon >= spatialSquare.WestLon)
            .AsNoTracking()
            .Select(candidate => candidate.Id.Value)
            .ToListAsync();
    }

    private async Task<List<User>> GetFinalCandidates(IEnumerable<Guid> initialCandidates)
    {
        // we want candidates sorted by number of likes descending
        return await _dbContext.Users
            .Where(candidate => initialCandidates.Contains(candidate.Id))
            .OrderByDescending(candidate => 
                _dbContext.Swipes.Where(swipe => swipe.SwipedWhoId.Equals(candidate.Id) && swipe.Like == Like.Like).Count())
            .Include(candidate => candidate.Settings)
            .Include(candidate => candidate.Photos)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<PublicUserDto>> HandleAsync(GetSwipeCandidates query)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Settings)
            .FirstOrDefaultAsync(user => user.Id.Equals(query.UserId));

        if (user == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        var initialCandidates = await GetInitialCandidatesToSwipe(user.Settings, query.UserId);
        var candidates = await GetFinalCandidates(initialCandidates);

        // we want candidates within range of user who requested
        return candidates
            .Select(candidate => 
                candidate.AsPublicDto(_spatial.CalculateDistanceInKms(user.Settings.Location.Lat, user.Settings.Location.Lon, 
                    candidate.Settings.Location.Lat, candidate.Settings.Location.Lon)))
            .Where(candidate => 
                candidate.DistanceInKms <= user.Settings.PreferredMaxDistance)
            .Take(query.HowMany)
            .ToList();
    }
}