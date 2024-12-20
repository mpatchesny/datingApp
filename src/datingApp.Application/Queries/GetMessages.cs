using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Core.Entities;

namespace datingApp.Application.Queries;

public class GetMessages : AuthenticatedPaginatedQuery<MessageDto>
{
    public Guid MatchId { get; set; }
}