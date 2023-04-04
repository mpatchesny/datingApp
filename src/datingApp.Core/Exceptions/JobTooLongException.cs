using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class JobTooLongException : CustomException
{
    public JobTooLongException() : base("Job title too long.")
    {
    }
}