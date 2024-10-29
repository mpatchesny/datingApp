using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public sealed record PhotoId
{
    public Guid Value { get; }

    public PhotoId(Guid value)
    {
        Value = value;
    }

    public static implicit operator Guid(PhotoId photoId) => photoId.Value;
    
    public static implicit operator PhotoId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}