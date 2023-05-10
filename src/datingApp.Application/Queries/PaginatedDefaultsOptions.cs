using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Queries;

public sealed class PaginatedDefaultsOptions
{
    public int DefaultPageSize { get; set; }
    public int MaxPageSize { get; set; }
}