using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;

namespace datingApp.Application.Queries;

public class PaginatedQuery
{
    public int Page { get; private set; } = 1;
    public int PageSize { get; private set; } = 15;

    private const int MaxPageSize = 100;

    public void SetPage(int? page)
    {
        if (page == null) return;
        if (page < 0)
        {
            throw new InvalidPageNumberException();
        }
        Page = (int) page;
    }

    public void SetPageSize(int? pageSize)
    {
        if (pageSize == null) return;
        if (pageSize < 0 || pageSize > MaxPageSize)
        {
            throw new InvalidPageSizeException(0, MaxPageSize);
        }
        PageSize = (int) pageSize;
    }
}