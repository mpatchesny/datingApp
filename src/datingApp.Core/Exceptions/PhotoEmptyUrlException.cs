using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class PhotoEmptyUrlException : CustomException
{
    public PhotoEmptyUrlException() : base($"Photo URL cannot be empty.")
    {
    }
}