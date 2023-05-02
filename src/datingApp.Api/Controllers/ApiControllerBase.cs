using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[Route("[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid AuthenticatedUserId => 
        !string.IsNullOrWhiteSpace(User.Identity?.Name) ? 
            Guid.Parse(User.Identity.Name) :
            Guid.Empty;

    protected dynamic Authenticate(dynamic commandOrQuery)
    {
        commandOrQuery.AuthenticatedUserId = this.AuthenticatedUserId;
        return commandOrQuery;
    }
}