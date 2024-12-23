using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using Xunit;

namespace datingApp.Tests.Unit.Queries;

public class AuthenticatedPaginatedQueryTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void given_Page_is_lower_or_equal_to_0_AuthenticatedPaginatedQueryTests_throws_InvalidPageNumberException(int page)
    {
        var query = new AuthenticatedPaginatedQuery<GetMatches>();
        var exception = Record.Exception(() => query.SetPage(page));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPageNumberException>(exception);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void given_PageSize_is_lower_than_0_or_greater_than_MaxPageSize_AuthenticatedPaginatedQueryTests_throws_InvalidPageSizeException(int pageSize)
    {
        var query = new AuthenticatedPaginatedQuery<GetMatches>();
        var exception = Record.Exception(() => query.SetPageSize(pageSize));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPageSizeException>(exception);
    }

    [Fact]
    public void given_passed_PageSize_is_null_PageSize_doesnt_change()
    {
        var query = new AuthenticatedPaginatedQuery<GetMatches>();
        int orgPageSize = query.PageSize;
        query.SetPageSize(null);
        Assert.Equal(orgPageSize, query.PageSize);
    }

    [Fact]
    public void given_passed_Page_is_null_Page_doesnt_change()
    {
        var query = new AuthenticatedPaginatedQuery<GetMatches>();
        int orgPage = query.Page;
        query.SetPage(null);
        Assert.Equal(orgPage, query.Page);
    }

    [Fact]
    public void SetPage_sets_page()
    {
        var query = new AuthenticatedPaginatedQuery<GetMatches>();
        query.SetPage(2);
        Assert.Equal(2, query.Page);
    }

    [Fact]
    public void SetPageSize_sets_page_size()
    {
        var query = new AuthenticatedPaginatedQuery<GetMatches>();
        query.SetPageSize(10);
        Assert.Equal(10, query.PageSize);
    }
}