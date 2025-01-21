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
        // Allowed chars: letters, spaces, hypens/ dashes, apostrophes
        // two consecutive special characters are not allowed
        char[] specialChars = ['-', ' ', '\''];

        for (int i = 1; i < value.Length; i++)
        {
            char c = value[i];
            char previousC = value[i-1];
            if (specialChars.Any(sc => sc == c) && c == previousC)
            {
                throw new InvalidUsernameException($"two special characters one after the other: {value}.");
            }
        }

        var invalidChars = !value.All(c => Char.IsLetter(c) || specialChars.Any(sc => sc == c));
        if (invalidChars)
        {
            throw new InvalidUsernameException($"contains forbidden characters {value}.");
        }

        Value = value;
    }

    public static implicit operator string(Name name)
        => name.Value;

    public static implicit operator Name(string value)
        => new(value);

    public override string ToString() => Value;
}