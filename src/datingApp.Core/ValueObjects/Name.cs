using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record Name
{
    private static readonly Regex BadNameRegex = new Regex(@"[^a-zA-Z\s]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

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

        if (BadNameRegex.IsMatch(value))
        {
            throw new InvalidUsernameException($"contains forbidden characters {value}");
        }

        Value = value;
    }

    public static implicit operator string(Name name)
        => name.Value;

    public static implicit operator Name(string value)
        => new(value);
}