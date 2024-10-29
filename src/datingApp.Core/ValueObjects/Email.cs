using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidEmailException("email address cannot be empty");
        }

        if (value.Length > 256)
        {
            throw new InvalidEmailException("email too long");
        }

        value = value.Trim().ToLowerInvariant();
        var emailAttrib = new EmailAddressAttribute();
        if (!emailAttrib.IsValid(value))
        {
            throw new InvalidEmailException($"invalid email address {value}");
        }

        Value = value;
    }

    public static implicit operator string(Email email)
        => email.Value;

    public static implicit operator Email(string value)
        => new(value);
}