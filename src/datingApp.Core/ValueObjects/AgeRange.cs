using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record AgeRange
{
    public int AgeFrom { get; private set; }
    public int AgeTo { get; private set; }

    public AgeRange(int ageFrom, int ageTo)
    {
        if (ageFrom < 18 | ageFrom > 100)
        {
            throw new InvalidDiscoveryAgeException("discover min age must be between 18 and 100");
        }
        else if (ageTo < 18 | ageTo > 100)
        {
            throw new InvalidDiscoveryAgeException("discover max age must be between 18 and 100");
        }
        else if (ageFrom > ageTo)
        {
            throw new InvalidDiscoveryAgeException("discover min age cannot be larger than max age");
        }
        AgeFrom = ageFrom;
        AgeTo = ageTo;
    }
}