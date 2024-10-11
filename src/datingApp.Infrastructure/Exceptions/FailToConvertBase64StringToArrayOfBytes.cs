using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

public class FailToConvertBase64StringToArrayOfBytesException : CustomException
{
    public FailToConvertBase64StringToArrayOfBytesException() : base("Provided image file is invalid or not in supported format.")
    {
    }
}