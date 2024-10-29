using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record Phone
{
    private static readonly Regex BadPhoneRegex = new Regex(@"[^0-9]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    public Phone(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidPhoneException("phone number cannot be empty");
        }

        if (value.Length > 9)
        {
            throw new InvalidPhoneException("phone number too long");
        }

        if (BadPhoneRegex.IsMatch(value))
        {
            throw new InvalidPhoneException("phone number must be only numbers");
        }

        Value = value;
    }

    public static implicit operator string(Phone phone)
        => phone.Value;

    public static implicit operator Phone(string value)
        => new(value);
}