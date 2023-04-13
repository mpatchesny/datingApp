using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class InvalidPageNumberException : CustomException
{
    public InvalidPageNumberException() : base($"Page number is invalid. Page number must be positive integer.")
    {
    }
}