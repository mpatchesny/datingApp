using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class Sandbox : IDisposable
{
    [Fact]
    public async Task Test1Async()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        for (int i = 0; i < 10; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            var messages = new List<Message>();
            for (int j = 0; j < 3; j++)
            {
                messages.Add(IntegrationTestHelper.CreateMessage(tempUser.Id));
            }
            for (int j = 0; j < 4; j++)
            {
                messages.Add(IntegrationTestHelper.CreateMessage(user1.Id));
            }
            var tempMatch = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id, messages: messages);
        }
        _dbContext.ChangeTracker.Clear();

        var messagesLimit = 10;

        // FIXME: drop anonymous view
        var userAndMatchQuery =
            from user in _dbContext.Users
                .Include(user => user.Settings)
                .Include(user => user.Photos)
            from match in _dbContext.Matches
                .Include(match => match.Messages
                    .OrderByDescending(message => message.CreatedAt)
                    .Take(messagesLimit))
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            select new 
            { 
                User = user,
                Match = match,
                OtherUserId = match.UserId1.Equals(user.Id) ? match.UserId2 : match.UserId1,
                MatchLastUpdateTime = MaxDate(match.Messages.FirstOrDefault().CreatedAt, match.CreatedAt),
                IsDisplayed = match.UserId1.Equals(user.Id) ? match.IsDisplayedByUser1 : match.IsDisplayedByUser2
            };

        var query =
            from userAndMatch in userAndMatchQuery
            from user in _dbContext.Users
                .Include(user => user.Settings)
            where user.Id.Equals(user1.Id)
            where user.Id.Equals(userAndMatch.OtherUserId)
            select new 
            { 
                User = user,
                Match = new
                {
                    User = userAndMatch.User,
                    Id = userAndMatch.Match.Id,
                    Messages = userAndMatch.Match.Messages,
                    IsDisplayed = userAndMatch.IsDisplayed,
                    CreatedAt = userAndMatch.Match.CreatedAt
                    // LastUpdateTime = userAndMatch.MatchLastUpdateTime
                }
            };

        var usersMatches = await 
            query.GroupBy(u => u.User, (key, group) => new { User = key, Matches = group.ToList() })
            .ToListAsync();

        Assert.Equal(1, usersMatches.Count());
    }

    private static DateTime MaxDate(DateTime createdAt1, DateTime createdAt2) 
        => createdAt1 > createdAt2 ? createdAt1 : createdAt2;

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    public Sandbox()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}