using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class UserPhotoLimitException : CustomException
{
    public UserPhotoLimitException() : base("User cannot have more than 6 photos.")
    {
    }
}