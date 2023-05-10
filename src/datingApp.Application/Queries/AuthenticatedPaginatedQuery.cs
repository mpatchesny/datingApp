using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;

namespace datingApp.Application.Queries;

public class AuthenticatedPaginatedQuery : AuthenticatedQueryBase<PaginatedDataDto>
{
    // FIXME: magic string
    public int Page { get; private set; } = 1;
    public int PageSize { get; private set; }
    private readonly PaginatedDefaultsOptions _options;

    public AuthenticatedPaginatedQuery(PaginatedDefaultsOptions options)
    {
        _options = options;
        if (PageSize == 0)
        {
            PageSize = _options.DefaultPageSize;
        }
    }

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
        if (pageSize < 0 || pageSize > _options.MaxPageSize)
        {
            throw new InvalidPageSizeException(0, _options.MaxPageSize);
        }
        PageSize = (int) pageSize;
    }
}