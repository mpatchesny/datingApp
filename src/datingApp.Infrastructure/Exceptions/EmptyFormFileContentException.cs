using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

public class EmptyFormFileContentException : CustomException
{
    public EmptyFormFileContentException() : base("Empty file.")
    {
    }
}