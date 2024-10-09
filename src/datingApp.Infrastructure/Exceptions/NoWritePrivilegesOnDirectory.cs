using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

public class NoWritePrivilegesOnDirectory : CustomException
{
    public NoWritePrivilegesOnDirectory(string message) : base(message)
    {
    }
}