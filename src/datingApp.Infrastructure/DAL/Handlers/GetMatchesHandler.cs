using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.DAL.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.IO;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMatchesHandler : IQueryHandler<GetMatches, PaginatedDataDto>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly DatingAppReadDbContext _readDbContext;
    public GetMatchesHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedDataDto> HandleAsync(GetMatches query)
    {
        int offset = (query.Page - 1) * query.PageSize;
        int limit = query.PageSize;

        var altQuery = await _readDbContext.Matches
            .Include(match => match.User)
            .Include(match => match.Messages.OrderByDescending(message => message.CreatedAt).Take(limit))
            .Where(match => match.User.Id == query.UserId)
            .Skip(offset)
            .Take(limit)
            .Select(match => match.AsDto())
            .ToListAsync();

        var dbQuery = GetMatchesQuery(query.UserId, limit: limit, offset: offset, messageLimit: 1, messageOffset: 0);
        var data = await dbQuery.ToListAsync();

        if (data.user == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        List<MatchDto> dataDto = new List<MatchDto>();
        foreach (var item in data)
        {
            dataDto.Add(
                new MatchDto()
                {
                    Id = item.Match.Id,
                    User = item.User.AsPublicDto(0),
                    IsDisplayed = item.Match.UserId1.Equals(query.UserId) ? item.Match.IsDisplayedByUser1 : item.Match.IsDisplayedByUser2,
                    Messages =  item.Match.MessagesAsDto(),
                    CreatedAt = item.Match.CreatedAt
                });
        }

        var recordsCount = await GetMatchesRecordsCountQuery(query.UserId).CountAsync();
        var pageCount = (int) (recordsCount + query.PageSize - 1) / query.PageSize;

        return new PaginatedDataDto
        {
            Page = query.Page,
            PageSize = query.PageSize,
            PageCount = pageCount,
            Data = new List<dynamic>(dataDto)
        };
    }

    private dynamic GetMatchesQuery(Guid userId, int limit, int offset, int messageLimit, int messageOffset)
    {
        var query = from match in _dbContext.Matches
                .Include(match => match.Messages
                    .OrderByDescending(message => message.CreatedAt)
                    .Skip(messageOffset)
                    .Take(messageLimit)
                    .OrderBy(message => message.CreatedAt))
            from user in _dbContext.Users.Include(user => user.Photos)
            where !user.Id.Equals(userId)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            select new 
            {
                Match = match,
                User = user
            };

        return query
            .AsNoTracking()
            .OrderByDescending(item => item.Match.CreatedAt)
            .Skip(offset)
            .Take(limit);
    }

    private dynamic GetMatchesRecordsCountQuery(Guid userId)
    {
        var query = from match in _dbContext.Matches
            from user in _dbContext.Users
            where !user.Id.Equals(userId)
            where match.UserId1.Equals(user.Id) || match.UserId2.Equals(user.Id)
            where match.UserId1.Equals(userId) || match.UserId2.Equals(userId)
            select new 
            {
                Match = match,
                User = user
            };
        return query.AsNoTracking();
    }
}