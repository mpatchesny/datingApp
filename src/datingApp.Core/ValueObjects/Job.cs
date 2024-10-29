using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record Job
{
    public string Value { get; }

    public Job(string value)
    {
        if (value.Length > 50)
        {
            throw new JobTooLongException();
        }
        Value = value;
    }

    public static implicit operator string(Job job)
        => job.Value;

    public static implicit operator Job(string value)
        => new(value);

    public override string ToString() => Value;
}