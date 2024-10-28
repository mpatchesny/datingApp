using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public sealed record MessageId
{
    public Guid Value { get; }

    public MessageId(Guid value)
    {
        Value = value;
    }

    public static implicit operator Guid(MessageId messageId) => messageId.Value;
    
    public static implicit operator MessageId(Guid value) => new(value);
}