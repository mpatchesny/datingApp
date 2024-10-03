using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class InvalidPhotoException : CustomException
{
    public InvalidPhotoException() : base("Provided image file is invalid or not in supported format.")
    {
    }
}