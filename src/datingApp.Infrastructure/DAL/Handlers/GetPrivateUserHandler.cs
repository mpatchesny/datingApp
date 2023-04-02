using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetPrivateUserHandler : IQueryHandler<GetPrivateUser, PrivateUserDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetPrivateUserHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<PrivateUserDto> HandleAsync(GetPrivateUser query)
    {
        var user= await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == query.UserId);
        return user?.AsPrivateDto();
    }
}