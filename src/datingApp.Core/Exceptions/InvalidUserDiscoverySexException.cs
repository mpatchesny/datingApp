using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class InvalidUserDiscoverySexException : CustomException
{
    public InvalidUserDiscoverySexException() : base("User preferred sex is invalid.")
    {
    }
}