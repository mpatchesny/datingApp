using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
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
        DateOnly minDob = new DateOnly(now.Year - settings.DiscoverAgeTo, now.Month, now.Day);
        DateOnly maxDob = new DateOnly(now.Year - settings.DiscoverAgeFrom, now.Month, now.Day);

        var spatialSquare = _spatial.GetApproxSquareAroundPoint(settings.Lat, settings.Lon, settings.DiscoverRange);

        var swipedCandidates = 
            _dbContext.Swipes.Where(s => s.SwipedById == userId).Select(x => x.SwipedWhoId);

        return _dbContext.Users
                    .Where(x => x.Id != userId)
                    .Where(x => !swipedCandidates.Contains(x.Id))
                    .Where(x => ((int) x.Sex & (int) settings.DiscoverSex) > 0)
                    .Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob)
                    .Where(x => x.Settings.Lat <= spatialSquare.NorthLat)
                    .Where(x => x.Settings.Lat >= spatialSquare.SouthLat)
                    .Where(x => x.Settings.Lon <= spatialSquare.EastLon)
                    .Where(x => x.Settings.Lon >= spatialSquare.WestLon)
                    .Select(x => x.Id);
    }

    private Task<List<User>> GetCandidatesAsync(IQueryable<Guid> initialCandidates, int howMany)
    {
        // we want candidates sorted by number of likes descending
        // we want only number of candidates equals to HowMany
        return _dbContext.Users
                    .Where(x => initialCandidates.Contains(x.Id))
                    .Select(x => new 
                        {
                            User = x,
                            LikesCount = _dbContext.Swipes.Where(s => s.SwipedWhoId == x.Id && s.Like == Like.Like).Count()
                        })
                    .OrderByDescending(x => x.LikesCount)
                    .Select(x => x.User)
                    .Include(x => x.Settings)
                    .Include(x => x.Photos)
                    .Take(howMany)
                    .AsNoTracking()
                    .ToListAsync();
    }

    public async Task<IEnumerable<PublicUserDto>> HandleAsync(GetSwipeCandidates query)
    {
        var settings = await _dbContext.UserSettings
                                .AsNoTracking()
                                .Where(x => x.UserId == query.UserId)
                                .FirstOrDefaultAsync();
        if (settings == null) return null;
        var initialCandidates = GetInitialCandidatesToSwipe(settings, query.UserId);
        var candidates = await GetCandidatesAsync(initialCandidates, query.HowMany);
        // we want candidates within range of user who requested
        return candidates
                .Select(u => u.AsPublicDto(_spatial.CalculateDistance(settings.Lat, settings.Lon, u.Settings.Lat, u.Settings.Lon)))
                .Where(u => u.Distance <= settings.DiscoverRange)
                .ToList();
    }
}