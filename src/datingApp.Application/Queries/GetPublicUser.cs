using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetPublicUser : AuthenticatedQueryBase<PublicUserDto>
{
    public Guid UserRequestedId { get; set; }
    public Guid UserId { get; set; }
}