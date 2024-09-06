using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class InvalidRefreshTokenException : CustomException
{
    public InvalidRefreshTokenException() : base("Provided refresh token is invalid.")
    {
    }

    public InvalidRefreshTokenException(string errMessage) : base(errMessage)
    {
    }
}