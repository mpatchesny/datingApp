using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class PhoneAlreadyInUseException : CustomException
{
    public PhoneAlreadyInUseException(string phone) : base($"Phone {phone} is already in use.")
    {
    }
}