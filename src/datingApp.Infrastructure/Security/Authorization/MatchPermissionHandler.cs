using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Application.Security.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace datingApp.Infrastructure.Security.Authorization;

// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0
// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-8.0
internal sealed class MatchPermissionHandler : AuthorizationHandler<IsOwnerRequirement, Match>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerRequirement requirement, Match resource)
    {
        if (context.User?.Identity?.Name == resource.UserId1.ToString() || context.User?.Identity?.Name == resource.UserId2.ToString())
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}