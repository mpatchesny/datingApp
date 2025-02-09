using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record Checksum
{
    public string Value { get; }

    public Checksum(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new EmptyChecksumException();
        }
        Value = value;
    }

    public static implicit operator string(Checksum checksum)
        => checksum.Value;

    public static implicit operator Checksum(string value)
        => new(value);

    public override string ToString() => Value;
}