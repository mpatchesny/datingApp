using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetIsLikedByOtherUserHandler : IQueryHandler<GetIsLikedByOtherUser, IsLikedByOtherUserDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetIsLikedByOtherUserHandler(DatingAppDbContext dbContext = null)
    {
        _dbContext = dbContext;
    }

    public async Task<IsLikedByOtherUserDto> HandleAsync(GetIsLikedByOtherUser query)
    {
        var likeExists = await _dbContext.Swipes
                                .AsNoTracking()
                                .AnyAsync(x => x.SwipedById == query.SwipedById && x.SwipedWhoId == query.SwipedWhoId && x.Like == Like.Like);
        
        return new IsLikedByOtherUserDto {
            IsLikedByOtherUser = likeExists
        };
    }
}