using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Security;
using Microsoft.AspNetCore.Authorization;

namespace datingApp.Infrastructure.Security;

public class AuthorizationService : IResourceAuthorizationService
{
    private readonly HttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizationService _authorizationService;
    public AuthorizationService(HttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationService = authorizationService;
    }

    public Task<AuthorizationResult> AuthorizeAsync(Guid userId, object resource, string policyName)
    {
        var user = _httpContextAccessor.HttpContext.User;
        if (user.Identity?.Name != userId.ToString())
        {
            // FIXME: czy to sie kiedykolwiek wydarzy?
            throw new UnauthorizedAccessException();
        }
        return _authorizationService.AuthorizeAsync(user, resource, policyName);
    }
}