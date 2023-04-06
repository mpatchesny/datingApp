using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetMatches : PaginatedQuery, IQuery<IEnumerable<MatchDto>>
{
    public int UserId { get; set; }
}