using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.ValueObjects;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class UserSettings
{
    public int UserId { get; private set; }
    public Sex DiscoverSex { get; private set; }
    public AgeRange DiscoverAgeRange { get; private set; }
    public int DiscoverRange { get; private set; }

    public UserSettings(int userId, Sex discoverSex, AgeRange discoverAgeRange, int discoverRange)
    {
        UserId = userId;
        SetDiscoverSex(discoverSex);
        SetDiscoverAge(discoverAgeRange);
        SetDiscoverRange(discoverRange);
    }

    public void ChangeDiscoverSex(Sex sex)
    {
        SetDiscoverSex(sex);
    }

    public void ChangeDiscoverAge(AgeRange discoverAgeRange)
    {
        SetDiscoverAge(discoverAgeRange);
    }

    public void ChangeDiscoverRange(int discoverRange)
    {
        SetDiscoverRange(discoverRange);
    }

    private void SetDiscoverSex(Sex sex)
    {
        if (DiscoverSex == sex) return;
        DiscoverSex = sex;
    }

    private void SetDiscoverAge(AgeRange discoverAgeRange)
    {
        if (DiscoverAgeRange == discoverAgeRange) return;
        DiscoverAgeRange = discoverAgeRange;
    }

    private void SetDiscoverRange(int discoverRange)
    {
        if (discoverRange < 1 | discoverRange > 100)
        {
            throw new InvalidDiscoveryRangeException();
        }
        if (DiscoverRange == discoverRange) return;
        DiscoverRange = discoverRange;
    }
}