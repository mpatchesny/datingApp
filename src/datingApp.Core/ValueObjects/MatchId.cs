using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public sealed record MatchId
{
    public Guid Value { get; }

    public MatchId(Guid value)
    {
        Value = value;
    }

    public static implicit operator Guid(MatchId matchId) => matchId.Value;
    
    public static implicit operator MatchId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}