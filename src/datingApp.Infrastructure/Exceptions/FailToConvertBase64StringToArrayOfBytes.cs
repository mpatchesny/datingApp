using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

public class FailToConvertBase64StringToArrayOfBytes : CustomException
{
    public FailToConvertBase64StringToArrayOfBytes() : base("Failed to convert base 64 string to array of bytes.")
    {
    }
}