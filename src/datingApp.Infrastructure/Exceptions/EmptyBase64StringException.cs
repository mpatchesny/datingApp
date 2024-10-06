using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

public class EmptyBase64StringException : CustomException
{
    public EmptyBase64StringException() : base("Base64 string is empty.")
    {
    }
}