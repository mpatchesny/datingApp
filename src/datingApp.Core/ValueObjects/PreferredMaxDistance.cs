using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record PreferredMaxDistance
{
    public int Value { get; }

    public PreferredMaxDistance(int value)
    {
        if (value < 1 | value > 100)
        {
            throw new InvalidDiscoveryRangeException();
        }
        Value = value;
    }

    public static implicit operator int(PreferredMaxDistance preferredMaxDistance)
        => preferredMaxDistance.Value;

    public static implicit operator PreferredMaxDistance(int value)
        => new(value);
}