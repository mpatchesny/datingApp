using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class BioTooLongException : CustomException
{
    public BioTooLongException() : base("User bio is too long.")
    {
    }
}