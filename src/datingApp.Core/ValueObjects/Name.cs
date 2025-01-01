using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record Name
{
    public string Value { get; }

    public Name(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidUsernameException("user name can't be empty");
        }

        if (value.Length > 15)
        {
            throw new InvalidUsernameException("user name too long");
        }

        // https://www.techiedelight.com/check-if-string-contains-only-letters-in-csharp/
        // Only letters, spaces and hypens are allowed
        if (!value.All(c => Char.IsLetter(c) || c == ' ' || c == '-'))
        {
            throw new InvalidUsernameException($"contains forbidden characters {value}");
        }

        Value = value;
    }

    public static implicit operator string(Name name)
        => name.Value;

    public static implicit operator Name(string value)
        => new(value);

    public override string ToString() => Value;
}