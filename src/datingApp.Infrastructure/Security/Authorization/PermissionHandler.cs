using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace datingApp.Infrastructure.Security.Authorization;

// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0
internal sealed class PermissionHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        // FIXME: przejrzeć, spr czy coś jeszcze potrzeba tutaj
        // żeby działało
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            switch (requirement)
            {
                case IsOwnerRequirement:
                    if (IsOwner(context.User, context.Resource)) 
                    {
                        context.Succeed(requirement);
                    }
                    break;
                default:
                    break;
            }
        }
        return Task.CompletedTask;
    }
    private static bool IsOwner(ClaimsPrincipal user, object? resource)
    {
        // TODO
        return true;
    }
}