using System;
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

    private IQueryable<Guid> GetInitialCandidatesToSwipe(UserSettings settings, Guid userId)
    {
        // we want only candidates who haven't been swiped by user who requested
        // we want candidates that matches sex, age and are within approx range of user who requested
        // we want candidates that are different from user who requested
        var now = DateTime.UtcNow;
        DateOnly minDob = new DateOnly(now.Year - settings.PreferredAge.To, now.Month, now.Day);
        DateOnly maxDob = new DateOnly(now.Year - settings.PreferredAge.From, now.Month, now.Day);

        var spatialSquare = _spatial.GetApproxSquareAroundPoint(settings.Location.Lat, settings.Location.Lon, settings.PreferredMaxDistance + 5);

        var swipedCandidates = 
            _dbContext.Swipes.Where(s => s.SwipedById.Equals(userId)).Select(x => x.SwipedWhoId.Value);

        int preferredSex = (int) settings.PreferredSex;

        return _dbContext.Users
                    .Where(candidate => candidate.Id != userId)
                    .Where(candidate => !swipedCandidates.Contains(candidate.Id))
                    .Where(candidate => ((int) candidate.Sex & preferredSex) != 0)
                    .Where(candidate => candidate.DateOfBirth >= minDob)
                    .Where(candidate => candidate.DateOfBirth <= maxDob)
                    .Where(x => x.Settings.Location.Lat <= spatialSquare.NorthLat)
                    .Where(x => x.Settings.Location.Lat >= spatialSquare.SouthLat)
                    .Where(x => x.Settings.Location.Lon <= spatialSquare.EastLon)
                    .Where(x => x.Settings.Location.Lon >= spatialSquare.WestLon)
                    .AsNoTracking()
                    .Select(candidate => candidate.Id);
    }

    private Task<List<User>> GetCandidatesAsync(IQueryable<Guid> initialCandidates)
    {
        // we want candidates sorted by number of likes descending
        return _dbContext.Users
                    .Where(x => initialCandidates.Contains(x.Id))
                    .Select(x => new 
                        {
                            User = x,
                            LikesCount = _dbContext.Swipes.Where(s => s.SwipedWhoId.Equals(x.Id) && s.Like == Like.Like).Count()
                        })
                    .OrderByDescending(x => x.LikesCount)
                    .Select(x => x.User)
                    .Include(x => x.Settings)
                    .Include(x => x.Photos)
                    .AsNoTracking()
                    .ToListAsync();
    }

    public async Task<IEnumerable<PublicUserDto>> HandleAsync(GetSwipeCandidates query)
    {
        if (!await _dbContext.Users.AnyAsync(x => x.Id == query.UserId))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var settings = await _dbContext.UserSettings
                                .Where(x => x.UserId.Equals(query.UserId))
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
        if (settings == null) return null;

        var initialCandidates = GetInitialCandidatesToSwipe(settings, query.UserId);
        var candidates = await GetCandidatesAsync(initialCandidates);

        // we want candidates within range of user who requested
        return candidates
                .Select(u => u.AsPublicDto(_spatial.CalculateDistanceInKms(settings.Location.Lat, settings.Location.Lon, u.Settings.Location.Lat, u.Settings.Location.Lon)))
                .Where(u => u.DistanceInKms <= settings.PreferredMaxDistance)
                .Take(query.HowMany)
                .ToList();
    }
}