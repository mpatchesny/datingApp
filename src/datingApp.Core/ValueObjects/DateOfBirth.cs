using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record DateOfBirth
{
    public DateOnly Value { get; }

    public DateOfBirth(DateOnly value)
    {
        if (value.CompareTo(new DateOnly()) == 0)
        {
            throw new DateOfBirthCannotBeEmptyException();
        }

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        var age = CalculateAge(value, currentDate);
        if (age < 18 | age > 100) 
        {
            throw new InvalidDateOfBirthException($"user cannot be younger than 18 or older than 100 years");
        }

        Value = value;
    }

    public static implicit operator DateOnly(DateOfBirth dateOfBirth)
        => dateOfBirth.Value;

    public static implicit operator DateOfBirth(DateOnly value)
        => new(value);

    public int GetAge()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        return CalculateAge(Value, currentDate);
    }

    private static int CalculateAge(DateOnly olderDate, DateOnly newerDate)
    {
        var age = newerDate.Year - olderDate.Year;

        switch (newerDate.Month - olderDate.Month)
        {
            case < 0:
                age -= 1;
                break;
            case 0:
                if (newerDate.Day < olderDate.Day)
                {
                    age -= 1;
                }
                break;
        }

        return age;
    }
}