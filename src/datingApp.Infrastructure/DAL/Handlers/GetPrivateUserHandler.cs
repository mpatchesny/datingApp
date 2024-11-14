using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Infrastructure.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetPrivateUserHandler : IQueryHandler<GetPrivateUser, PrivateUserDto>
{
    private readonly DatingAppDbContext _dbContext;
    private readonly DatingAppReadDbContext _readDbContext;
    public GetPrivateUserHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PrivateUserDto> HandleAsync(GetPrivateUser query)
    {
        var altUser = await _readDbContext.Users
                    .Include(user => user.Settings)
                    .Include(user => user.Photos)
                    .Where(user => user.Id.Equals(query.UserId))
                    .Select(user => user.AsPrivateDto())
                    .FirstOrDefaultAsync();

        var user = await _dbContext.Users
                                .AsNoTracking()
                                .Include(user => user.Settings)
                                .Include(user => user.Photos)
                                .Where(user => user.Id.Equals(query.UserId))
                                .Select(user => user.AsPrivateDto())
                                .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotExistsException(query.UserId);
        }

        return user;
    }
}