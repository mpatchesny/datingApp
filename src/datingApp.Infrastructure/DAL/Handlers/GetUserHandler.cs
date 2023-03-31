using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetUserHandler : IQueryHandler<GetUser, PublicUserDto>
{
    private readonly IUserRepository _userRepository;
    public GetUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PublicUserDto> HandleAsync(GetUser query)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId);
        return user?.AsDto();
    }
}