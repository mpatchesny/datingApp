using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Security.Authorization;
using datingApp.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace datingApp.Infrastructure.Security.Authorization;

internal sealed class UserPermissionHandler : AuthorizationHandler<IsOwnerRequirement, User>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerRequirement requirement, User resource)
    {
        if (context.User?.Identity?.Name == resource.Id.ToString())
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}