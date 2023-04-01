using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetPublicUserHandler : IQueryHandler<GetPublicUser, PublicUserDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetPublicUserHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PublicUserDto> HandleAsync(GetPublicUser query)
    {
        var user= await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == query.UserId);
        return user?.AsPublicDto();
    }
}