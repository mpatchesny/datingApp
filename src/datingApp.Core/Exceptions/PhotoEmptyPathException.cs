using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class PhotoEmptyPathException : CustomException
{
    public PhotoEmptyPathException() : base($"Photo path cannot be empty.")
    {
    }
}