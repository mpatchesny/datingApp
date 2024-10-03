using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class EmptyPhotoUrlException : CustomException
{
    public EmptyPhotoUrlException() : base($"Photo URL cannot be empty.")
    {
    }
}