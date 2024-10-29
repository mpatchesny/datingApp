using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public sealed record UserId
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        Value = value;
    }

    public static implicit operator Guid(UserId userId) => userId.Value;
    
    public static implicit operator UserId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}