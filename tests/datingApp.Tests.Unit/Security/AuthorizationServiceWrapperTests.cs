using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Unit.Security;

public class AuthorizationServiceWrapperTests
{
    [Fact]
    public async void given_resource_is_null_authorization_throws_UnauthorizedException()
    {
        var userId = Guid.NewGuid();
        var exception = await Record.ExceptionAsync(() => _authorizationServiceWrapper.AuthorizeAsync(userId, null, "some policy"));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async void given_resource_is_not_null_and_IHttpContextAccessor_context_user_name_not_equals_user_id_authorization_throws_UnauthorizedException()
    {
        var userId = Guid.NewGuid();
        var user = CreateClaimsPrincipal(userId);
        _httpContextAccessor.Setup(x => x.HttpContext.User).Returns(user);
        var someObject = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        var badUserId = Guid.NewGuid();

        var exception = await Record.ExceptionAsync(() => _authorizationServiceWrapper.AuthorizeAsync(badUserId, someObject, "some policy"));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_resource_is_not_null_and_IHttpContextAccessor_context_user_name_equals_user_id_authorization_calls_IAuthorizationService_once()
    {
        var userId = Guid.NewGuid();
        var user = CreateClaimsPrincipal(userId);
        _httpContextAccessor.Setup(x => x.HttpContext.User).Returns(user);
        var someObject = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false, false, null);

        var exception = await Record.ExceptionAsync(() => _authorizationServiceWrapper.AuthorizeAsync(userId, someObject, "some policy"));
        Assert.Null(exception);
        _authorizationService.Verify(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), someObject, "some policy"), Times.Once);
    }

    private readonly IDatingAppAuthorizationService _authorizationServiceWrapper;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly Mock<IAuthorizationService> _authorizationService;

    private static ClaimsPrincipal CreateClaimsPrincipal(Guid userId)
    {
        var claims = new List<Claim>
            {
                new("sub", userId.ToString()),
                new("name", userId.ToString()),
            };
        var identity = new ClaimsIdentity(claims, authenticationType: string.Empty, nameType: "sub", roleType: string.Empty);
        return new ClaimsPrincipal(identity);
    }

    public AuthorizationServiceWrapperTests()
    {
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _authorizationService = new Mock<IAuthorizationService>();
        _authorizationServiceWrapper = new AuthorizationServiceWrapper(_httpContextAccessor.Object, _authorizationService.Object);
    }
}