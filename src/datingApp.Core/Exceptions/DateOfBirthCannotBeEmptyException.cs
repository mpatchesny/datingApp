using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public sealed class DateOfBirthCannotBeEmptyException : CustomException
{
    public DateOfBirthCannotBeEmptyException() : base($"Date of birth cannot be empty.")
    {
    }
}