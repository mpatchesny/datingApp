using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class PhotoAlreadyExistsException : CustomException
{
    public PhotoAlreadyExistsException() : base("Photo already exists for this user.")
    {
    }
}
