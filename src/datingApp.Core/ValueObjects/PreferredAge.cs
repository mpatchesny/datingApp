using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record PreferredAge
{
    public int From { get; }
    public int To { get; }

    public PreferredAge(int from, int to)
    {
        if (from < 18 | to > 100)
        {
            throw new InvalidDiscoveryAgeException("minimum preferred age must be between 18 and 100");
        }
        else if (to < 18 | to > 100)
        {
            throw new InvalidDiscoveryAgeException("maximum preferred age must be between 18 and 100");
        }
        else if (from > to)
        {
            throw new InvalidDiscoveryAgeException("minimum preferred age cannot be larger than maximum preferred age");
        }

        From = from;
        To = to;
    }

    public override string ToString() => $"{From} -> {To}";
}