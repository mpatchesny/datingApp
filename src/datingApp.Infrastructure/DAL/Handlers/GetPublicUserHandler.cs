using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetPublicUserHandler : IQueryHandler<GetPublicUser, PublicUserDto>
{
    private readonly IUserRepository _userRepository;
    public GetPublicUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PublicUserDto> HandleAsync(GetPublicUser query)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId);
        return user?.AsDto();
    }
}