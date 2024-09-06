using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class RefreshTokenRevokedException : InvalidRefreshTokenException
{
    public RefreshTokenRevokedException() : base("Provided refresh token has been revoked and is no longer accepted.")
    {
    }
}