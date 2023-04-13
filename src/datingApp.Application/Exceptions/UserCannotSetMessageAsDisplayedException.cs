using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class UserCannotSetMessageAsDisplayedException : CustomException
{
    public UserCannotSetMessageAsDisplayedException(Guid messageId) : base($"Only user who received message can mark it as displayed.")
    {
    }
}