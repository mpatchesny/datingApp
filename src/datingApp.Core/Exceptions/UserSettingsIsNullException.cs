using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class UserSettingsIsNullException : CustomException
{
    public UserSettingsIsNullException() : base("User settings cannot be null.")
    {
    }
}