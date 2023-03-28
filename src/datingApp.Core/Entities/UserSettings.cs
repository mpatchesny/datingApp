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
    public int DiscoverAgeFrom { get; private set; }
    public int DiscoverAgeTo { get; private set; }
    public int DiscoverRange { get; private set; }

    private UserSettings()
    {
        // EF
    }

    public UserSettings(int userId, Sex discoverSex, int discoverAgeFrom, int discoverAgeTo, int discoverRange)
    {
        UserId = userId;
        SetDiscoverSex(discoverSex);
        SetDiscoverAge(discoverAgeFrom, discoverAgeTo);
        SetDiscoverRange(discoverRange);
    }

    public void ChangeDiscoverSex(Sex sex)
    {
        SetDiscoverSex(sex);
    }

    public void ChangeDiscoverAge(int discoverAgeFrom, int discoverAgeTo)
    {
        SetDiscoverAge(discoverAgeFrom, discoverAgeTo);
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

    private void SetDiscoverAge(int discoverAgeFrom, int discoverAgeTo)
    {
        if (discoverAgeFrom < 18 | discoverAgeFrom > 100)
        {
            throw new InvalidDiscoveryAgeException("discover min age must be between 18 and 100");
        }
        else if (discoverAgeTo < 18 | discoverAgeTo > 100)
        {
            throw new InvalidDiscoveryAgeException("discover max age must be between 18 and 100");
        }
        else if (discoverAgeFrom > discoverAgeTo)
        {
            throw new InvalidDiscoveryAgeException("discover min age cannot be larger than max age");
        }
        DiscoverAgeFrom = discoverAgeFrom;
        DiscoverAgeTo = discoverAgeTo;
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