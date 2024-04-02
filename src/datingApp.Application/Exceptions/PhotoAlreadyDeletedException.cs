using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class PhotoAlreadyDeletedException : CustomException
{
    public PhotoAlreadyDeletedException() : base($"Photo is deleted permanently.")
    {
    }
}