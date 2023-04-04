using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class InvalidDateOfBirthException : CustomException
{
    public InvalidDateOfBirthException(string details) : base($"Invalid date of birth: {details}.")
    {
    }
}