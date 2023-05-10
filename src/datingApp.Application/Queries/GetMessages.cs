using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetMessages : AuthenticatedPaginatedQuery
{
    public GetMessages(PaginatedDefaultsOptions options) : base(options)
    {
    }

    public Guid MatchId { get; set; }
}