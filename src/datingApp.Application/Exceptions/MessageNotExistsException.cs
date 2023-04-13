using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class MessageNotExistsException : CustomException
{
    public MessageNotExistsException(Guid messageId) : base($"Message with id {messageId} does not exist.")
    {
    }
}