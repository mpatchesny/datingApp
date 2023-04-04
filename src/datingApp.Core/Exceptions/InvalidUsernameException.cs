using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public sealed class InvalidUsernameException : CustomException
{
    public InvalidUsernameException(string details) : base($"User name is invalid: {details}.")
    {
    }
}