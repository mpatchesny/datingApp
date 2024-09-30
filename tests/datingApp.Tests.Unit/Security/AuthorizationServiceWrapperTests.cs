using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Security;
using datingApp.Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Security;

public class AuthorizationServiceWrapperTests
{
    [Fact]
    public void given_resource_is_null_authorization_throws_UnauthorizedException()
    {
        // TODO
    }

    [Fact]
    public void given_IHttpContextAccessor_context_user_name_not_equals_user_id_authorization_throws_UnauthorizedException()
    {
        // TODO
    }

    [Fact]
    public void given_resource_is_not_null_and_IHttpContextAccessor_context_user_name_equals_user_id_authorization_calls_IAuthorizationService_once()
    {
        // TODO
    }

    private readonly IDatingAppAuthorizationService _authorizationServiceWrapper;

    public AuthorizationServiceWrapperTests()
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var authorizationService = new Mock<IAuthorizationService>();
        _authorizationServiceWrapper = new AuthorizationServiceWrapper(httpContextAccessor.Object, authorizationService.Object);
    }
}