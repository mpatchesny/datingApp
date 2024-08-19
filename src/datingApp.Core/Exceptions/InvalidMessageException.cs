using System.Runtime.Serialization;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public sealed class InvalidMessageException : CustomException
{
    public InvalidMessageException(string details) : base($"Message is invalid: {details}.")
    {
    }
}