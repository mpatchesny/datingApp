using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;

namespace datingApp.Application.Queries;

public class PaginatedQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}