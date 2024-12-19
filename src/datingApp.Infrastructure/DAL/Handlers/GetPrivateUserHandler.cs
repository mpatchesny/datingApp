using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetPrivateUserHandler : IQueryHandler<GetPrivateUser, PrivateUserDto>
{
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    public GetPrivateUserHandler(ReadOnlyDatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<PrivateUserDto> HandleAsync(GetPrivateUser query)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Settings)
            .Include(user => user.Photos)
            .FirstOrDefaultAsync(user => user.Id.Equals(query.UserId));

        if (user == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        return user.AsPrivateDto();
    }
}