using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class UserLocationIsNullException : CustomException
{
    public UserLocationIsNullException() : base("user location must be set")
    {
    }
}