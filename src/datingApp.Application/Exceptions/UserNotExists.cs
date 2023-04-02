using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class UserNotExists : CustomException
{
    public UserNotExists(int userId) : base($"User with id {userId} does not exist")
    {
    }
}