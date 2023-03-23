using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class InvalidLocationException : CustomException
{
    public InvalidLocationException() : base("Location is invalid")
    {
    }
}