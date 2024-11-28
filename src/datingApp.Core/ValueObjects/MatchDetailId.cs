using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public record MatchDetailId
{
    public Guid Value { get; }

    public MatchDetailId(Guid value)
    {
        Value = value;
    }

    public static implicit operator Guid(MatchDetailId matchId) => matchId.Value;
    
    public static implicit operator MatchDetailId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}