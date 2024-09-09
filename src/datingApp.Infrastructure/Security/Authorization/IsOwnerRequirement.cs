using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace datingApp.Infrastructure.Security.Authorization;

internal sealed class IsOwnerRequirement : IAuthorizationRequirement
{
    public IsOwnerRequirement()
    {
        // TODO: sprawdzić, czy Requirement musi mieć jakieś dane
        // to jest chyba coś jak Command/Query
    }
}