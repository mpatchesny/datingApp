using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetSwipeCandidates : AuthenticatedQueryBase<IEnumerable<PublicUserDto>>
{
    public Guid UserId { get; set; }
    public int HowMany { get; set; } = 10;
}