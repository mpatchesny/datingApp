using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class NotExistsException : CustomException
{
    public NotExistsException(string message) : base(message)
    {
    }
}