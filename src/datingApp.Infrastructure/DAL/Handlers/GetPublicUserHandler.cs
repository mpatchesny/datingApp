using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Infrastructure.Spatial;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetPublicUserHandler : IQueryHandler<GetPublicUser, PublicUserDto>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;
    public GetPublicUserHandler(DatingAppDbContext dbContext, ISpatial spatial)
    {
        _dbContext = dbContext;
        _spatial = spatial;
    }

    public async Task<PublicUserDto> HandleAsync(GetPublicUser query)
    {
        var userRequested = await _dbContext.UserSettings
                                        .AsNoTracking()
                                        .Where(x => x.UserId == query.UserRequestedId)
                                        .Select(x => new { x.Lat, x.Lon })
                                        .FirstOrDefaultAsync();

        if (userRequested == null) return null;

        var user = await _dbContext.Users
                            .AsNoTracking()
                            .Include(x => x.Settings)
                            .Include(x => x.Photos)
                            .FirstOrDefaultAsync(x => x.Id == query.UserId);
                            
        return user?.AsPublicDto(_spatial.CalculateDistance(userRequested.Lat, userRequested.Lon, user.Settings.Lat, user.Settings.Lon));
    }
}