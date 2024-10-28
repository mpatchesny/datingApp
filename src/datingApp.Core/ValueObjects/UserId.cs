using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public class UserId
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        Value = value;
    }

    public static implicit operator Guid(UserId userId) => userId.Value;
    
    public static implicit operator UserId(Guid value) => new(value);
}