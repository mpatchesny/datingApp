using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public sealed class InvalidEmailException : CustomException
{
    public InvalidEmailException(string details) : base($"Email address is invalid: {details}.")
    {
    }
}