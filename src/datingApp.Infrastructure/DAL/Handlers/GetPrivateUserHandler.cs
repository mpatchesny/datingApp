using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetPrivateUserHandler : IQueryHandler<GetPrivateUser, PrivateUserDto>
{
    private readonly IUserRepository _userRepository;
    public GetPrivateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<PrivateUserDto> HandleAsync(GetPrivateUser query)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId);
        return user?.AsPrivateDto();
    }
}