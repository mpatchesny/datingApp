using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;

namespace datingApp.Application.Queries;

public class GetNotDisplayedMatchesAndMessages : AuthenticatedQueryBase<Tuple<int, int>>
{
    public Guid UserId { get; set; }
}