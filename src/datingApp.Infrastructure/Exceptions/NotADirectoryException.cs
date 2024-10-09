using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

public class NotADirectoryException : CustomException
{
    public NotADirectoryException(string message) : base(message)
    {
    }
}