using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.Spatial;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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

    private IIncludableQueryable<User, ICollection<Photo>> GetCandidatesToSwipeQuery(UserSettings settings, Guid requestedById)
    {
        // we want only candidates who haven't been swiped by user who requested
        // we want candidates that matches sex, age and are within approx range of user who requested
        // we want candidates that are different from user who requested
        var now = DateTime.UtcNow;
        var minDob = new DateOnly(now.Year - settings.PreferredAge.To, now.Month, now.Day);
        var maxDob = new DateOnly(now.Year - settings.PreferredAge.From, now.Month, now.Day);

        int preferredSex = (int)settings.PreferredSex;

        var spatialSquare = _spatial.GetApproxSquareAroundPoint(settings.Location.Lat, settings.Location.Lon,
            settings.PreferredMaxDistance + 5);

        var alreadySwiped = _dbContext.Swipes
            .Where(swipe => swipe.SwipedById.Equals(requestedById))
            .Select(swipe => swipe.SwipedWhoId);

        return _dbContext.Users
            .AsNoTracking()
            .Where(candidate => !candidate.Id.Equals(requestedById))
            .Where(candidate => !alreadySwiped.Contains(candidate.Id))
            .Where(candidate => ((int)candidate.Sex & preferredSex) != 0)
            .Where(candidate => candidate.DateOfBirth >= minDob)
            .Where(candidate => candidate.DateOfBirth <= maxDob)
            .Where(candidate => candidate.Settings.Location.Lat <= spatialSquare.NorthLat)
            .Where(candidate => candidate.Settings.Location.Lat >= spatialSquare.SouthLat)
            .Where(candidate => candidate.Settings.Location.Lon <= spatialSquare.EastLon)
            .Where(candidate => candidate.Settings.Location.Lon >= spatialSquare.WestLon)
            .OrderByDescending(candidate =>
                _dbContext.Swipes.Where(swipe => swipe.SwipedWhoId.Equals(candidate.Id)
                    && swipe.Like == Like.Like).Count())
            .Include(candidate => candidate.Settings)
            .Include(candidate => candidate.Photos);
    }

    private async Task<List<PublicUserDto>> GetCandidatesAsync(User requestedBy, int howMany)
    {
        var candidatesQuery = GetCandidatesToSwipeQuery(requestedBy.Settings, requestedBy.Id);

        var limit = howMany * 2;
        var offset = 0;

        var finalCandidates = new List<PublicUserDto>();
        while (finalCandidates.Count < howMany)
        {
            var pontentialCandidates =
                await candidatesQuery.Skip(offset).Take(limit).ToListAsync();

            var pontentialCandidatesWithinRange =
                from candidate in pontentialCandidates
                let distance = _spatial.CalculateDistanceInKms(
                    requestedBy.Settings.Location.Lat,
                    requestedBy.Settings.Location.Lon,
                    candidate.Settings.Location.Lat,
                    candidate.Settings.Location.Lon)
                where distance <= requestedBy.Settings.PreferredMaxDistance
                select candidate.AsPublicDto(distance);

            if (!pontentialCandidates.Any()) break;

            finalCandidates.AddRange(pontentialCandidatesWithinRange);
            offset += limit;
        }

        return finalCandidates.Take(howMany).ToList();
    }

    public async Task<IEnumerable<PublicUserDto>> HandleAsync(GetSwipeCandidates query)
    {
        var requestedBy = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Settings)
            .FirstOrDefaultAsync(user => user.Id.Equals(query.UserId));

        if (requestedBy == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        return await GetCandidatesAsync(requestedBy, query.HowMany);
    }


}