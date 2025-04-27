using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetNotDisplayedMatchesAndMessagesHandler
{
    private ReadOnlyDatingAppDbContext _dbContext;
    public GetNotDisplayedMatchesAndMessagesHandler(ReadOnlyDatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tuple<int, int>> HandleAsync(GetNotDisplayedMatchesAndMessages query)
    {
        if (!await _dbContext.Users.AnyAsync(user => user.Id.Equals(query.UserId)))
        {
            throw new UserNotExistsException(query.UserId);
        }

        var notDisplayedMessagesCountQuery = _dbContext.Matches
            .Where(match => match.Users
                .Any(user => user.Id.Equals(query.UserId)))
            .Where(match => match.Messages.All(message =>
                !message.IsDisplayed && !message.SendFromId.Equals(query.UserId)))
            .CountAsync();

        var notDisplayedMatchesCountQuery = _dbContext.Matches
            .Where(match => match.Users
                .Any(user => user.Id.Equals(query.UserId)))
            .Where(match => match.MatchDetails.Any(matchDetail => !matchDetail.IsDisplayed 
                && !matchDetail.UserId.Equals(query.UserId)))
            .CountAsync();

        var notDisplayedMessagesCount = await notDisplayedMessagesCountQuery;
        var notDisplayedMatchesCount = await notDisplayedMatchesCountQuery;

        return new Tuple<int, int>(notDisplayedMatchesCount, notDisplayedMessagesCount);
    }
}