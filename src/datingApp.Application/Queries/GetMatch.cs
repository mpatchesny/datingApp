using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetMatch : AuthenticatedQueryBase<MatchDto>
{
    public Guid MatchId { get; set; }
    public Guid UserId { get; set; }
}