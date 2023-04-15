using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class UserNotExistsException : CustomException
{
    public UserNotExistsException() : base($"User with given phone/email does not exist.")
    {
    }
    public UserNotExistsException(Guid userId) : base($"User with id {userId} does not exist.")
    {
    }

}