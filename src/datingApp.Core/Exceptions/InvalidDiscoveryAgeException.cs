using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class InvalidDiscoveryAgeException : CustomException
{
    public InvalidDiscoveryAgeException(string details) : base($"Invalid preferred age: {details}.")
    {
    }
}