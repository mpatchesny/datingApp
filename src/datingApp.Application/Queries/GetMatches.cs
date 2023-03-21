using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;

namespace datingApp.Application.Queries;

public class GetMatches : IQuery<IEnumerable<object>>
{
    public int UserId { get; set; }
}