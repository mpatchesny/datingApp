using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public sealed record Oridinal
{
    public int Value { get; }

    public Oridinal(int value)
    {
        if (value <= -1)
        {
            value = 0;
        }

        Value = value;
    }

    public static implicit operator int(Oridinal oridinal)
        => oridinal.Value;

    public static implicit operator Oridinal(int value)
        => new(value);
}