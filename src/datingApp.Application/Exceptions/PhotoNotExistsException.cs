using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class PhotoNotExistsException : CustomException
{
    public PhotoNotExistsException(Guid photoId) : base($"Photo with id {photoId} does not exist.")
    {
    }
}