using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class EmptyChecksumException : CustomException
{
    public EmptyChecksumException() : base("Checksum cannot be empty.")
    {
    }
}