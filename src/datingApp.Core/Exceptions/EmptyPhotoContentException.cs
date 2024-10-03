using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class EmptyPhotoContentException : CustomException
{
    public EmptyPhotoContentException() : base("Photo content cannot be empty.")
    {
    }
}