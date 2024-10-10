using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

public class DirectoryNotExistsException : CustomException
{
    public DirectoryNotExistsException(string message) : base(message)
    {
    }
}