using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public class LikesCount
{
    public int Value { get; }

    public LikesCount(int value)
    {
        if (value <= -1)
        {
            value = 0;
        }

        Value = value;
    }

    public static implicit operator int(LikesCount oridinal)
        => oridinal.Value;

    public static implicit operator LikesCount(int value)
        => new(value);
}