using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class InvalidEmailException : CustomException
{
    public InvalidEmailException(string details) : base($"Email address is invalid: {details}.")
    {
    }
}