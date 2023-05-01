using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

public class UnauthorizedException : CustomException
{
    public UnauthorizedException() : base("Unauthorized")
    {
    }
}