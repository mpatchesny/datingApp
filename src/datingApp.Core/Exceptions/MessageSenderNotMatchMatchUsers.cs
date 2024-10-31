using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public sealed class MessageSenderNotMatchMatchUsers : CustomException
{
    public MessageSenderNotMatchMatchUsers() : base("Messege sender does not match any of the Match users.")
    {
    }
}