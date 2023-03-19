using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class InvalidPhoneException : CustomException
{
    public InvalidPhoneException(string details) : base($"Phone number is invalid: {details}")
    {
    }
}