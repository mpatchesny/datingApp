using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Security;
using Microsoft.AspNetCore.Authorization;

namespace datingApp.Infrastructure.Security.Authorization;

internal sealed class AuthorizationServiceWrapper : IDatingAppAuthorizationService
{
    private readonly HttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizationService _authorizationService;
    public AuthorizationServiceWrapper(HttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationService = authorizationService;
    }

    public Task<AuthorizationResult> AuthorizeAsync(Guid userId, object resource, string policyName)
    {
        var user = _httpContextAccessor.HttpContext.User;
        if (user?.Identity?.Name != userId.ToString())
        {
            // FIXME: czy to sie kiedykolwiek wydarzy?
            // FIXME: zmienić exception na inny, np. authenticated user different than context user exception???
            throw new UnauthorizedAccessException();
        }
        return _authorizationService.AuthorizeAsync(user, resource, policyName);
    }
}