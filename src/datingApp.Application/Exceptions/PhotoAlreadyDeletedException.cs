using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class PhotoAlreadyDeletedException : AlreadyDeletedException
{
    public PhotoAlreadyDeletedException(Guid photoId) : base($"Photo {photoId} is deleted permanently.")
    {
    }
}