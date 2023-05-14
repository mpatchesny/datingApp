using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class InvalidUserSexException : CustomException
{
    public InvalidUserSexException() : base("User sex is invalid.")
    {
    }
}