using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace datingApp.Application.Security;

public interface IResourceAuthorizationService
{
    public Task<AuthorizationResult> AuthorizeAsync(Guid userId, object resource, string policyName);
}