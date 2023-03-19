using System.Runtime.Serialization;

namespace datingApp.Core.Entities;

public sealed class InvalidMessageException : Exception
{
    public InvalidMessageException(string details) : base($"Message is invalid: {details}")
    {
    }
}