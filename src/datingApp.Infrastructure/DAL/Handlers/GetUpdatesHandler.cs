using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetUpdatesHandler : IQueryHandler<GetUpdates, IEnumerable<MatchDto>>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly ISpatial _spatial;

    public GetUpdatesHandler(DatingAppDbContext dbContext, ISpatial spatial)
    {
        _dbContext = dbContext;
        _spatial = spatial;
    }

    public async Task<IEnumerable<MatchDto>> HandleAsync(GetUpdates query)
    {
        if (!await _dbContext.Users.AnyAsync(user => user.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var dbQuery = 
            from match in _dbContext.Matches
            .Include(match => match.Messages
                .Where(message => message.CreatedAt >= query.LastActivityTime))
            .Where(match => 
                match.Messages.Any(message => message.CreatedAt >= query.LastActivityTime) ||
                match.CreatedAt >= query.LastActivityTime)
            .Include(match => match.Users)
                .ThenInclude(user => user.Photos)
            .Include(match => match.Users)
                .ThenInclude(user => user.Settings)
            .Where(match => match.Users
                .Any(user => user.Id.Equals(query.UserId)))
            select match;

        var matches = await dbQuery.AsNoTracking().ToListAsync();

        List<MatchDto> dataDto = new List<MatchDto>();
        foreach (var match in matches)
        {
            var user1 = match.Users.ElementAt(0);
            var user2 = match.Users.ElementAt(1);

            var distanceInKms = _spatial.CalculateDistanceInKms(user1, user2);

            var userDto = user1.Id.Equals(query.UserId) ? 
                user2.AsPublicDto(distanceInKms) :
                user1.AsPublicDto(distanceInKms);

            dataDto.Add(
                new MatchDto()
                {
                    Id = match.Id,
                    User = userDto,
                    IsDisplayed = match.IsDisplayedByUser(query.UserId),
                    Messages =  match.MessagesAsDto(),
                    CreatedAt = match.CreatedAt
                });
        }

        return dataDto;
    }
}