using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class InvalidDateOfBirthFormatException : CustomException
{
    public InvalidDateOfBirthFormatException(string date) : base($"Provided date of birth: {date} is invalid.")
    {
    }
}