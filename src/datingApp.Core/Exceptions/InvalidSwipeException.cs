using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class InvalidSwipeException : CustomException
{
    public InvalidSwipeException() : base("User cannot swipe himself/herself")
    {
    }
}