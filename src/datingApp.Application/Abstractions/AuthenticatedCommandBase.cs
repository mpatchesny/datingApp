using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Abstractions;

public record AuthenticatedCommandBase() : IAuthenticatedCommand
{
    public Guid AuthenticatedUserId { get ; set; }
}