using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class InvalidDiscoveryRangeException : CustomException
{
    public InvalidDiscoveryRangeException() : base("discovery range must be between 1 and 100")
    {
    }
}