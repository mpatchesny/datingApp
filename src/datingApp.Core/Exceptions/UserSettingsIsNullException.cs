using System.Runtime.Serialization;

namespace datingApp.Core.Exceptions;

public sealed class UserSettingsIsNullException : CustomException
{
    public UserSettingsIsNullException() : base("user settings cannot be null")
    {
    }
}