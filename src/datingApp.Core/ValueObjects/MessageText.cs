using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.ValueObjects;

public sealed record MessageText
{
    string Value { get; }

    public MessageText(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidMessageException("message cannot be empty");
        }
        if (value.Length > 255)
        {
            throw new InvalidMessageException("message too long");
        }
        Value = value;
    }

    public static implicit operator string(MessageText text)
        => text.Value;

    public static implicit operator MessageText(string value)
        => new(value);
}