using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class EmailAlreadyInUseException : CustomException
{
    public EmailAlreadyInUseException(string email) : base($"Email {email} is already in use.")
    {
    }
}