using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class UserAlreadyDeletedException : CustomException
{
    public UserAlreadyDeletedException() : base($"User is deleted permanently.")
    {
    }
}