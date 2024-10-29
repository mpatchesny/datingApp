using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record Bio
{
    public string Value { get; }

    public Bio(string value)
    {
        if (value.Length > 400)
        {
            throw new BioTooLongException();
        }
        Value = value;
    }

    public static implicit operator string(Bio bio)
        => bio.Value;

    public static implicit operator Bio(string value)
        => new(value);

    public override string ToString() => Value;
}