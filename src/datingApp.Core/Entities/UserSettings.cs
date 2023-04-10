using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class UserSettings
{
    public Guid UserId { get; private set; }
    public Sex DiscoverSex { get; private set; }
    public int DiscoverAgeFrom { get; private set; }
    public int DiscoverAgeTo { get; private set; }
    public int DiscoverRange { get; private set; }
    public double Lat { get; private set; }
    public double Lon { get; private set; }

    public UserSettings(Guid userId, Sex discoverSex, int discoverAgeFrom, int discoverAgeTo, int discoverRange, double lat, double lon)
    {
        UserId = userId;
        SetDiscoverSex(discoverSex);
        SetDiscoverAge(discoverAgeFrom, discoverAgeTo);
        SetDiscoverRange(discoverRange);
        SetLocation(lat, lon);
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

    public void ChangeLocation(double lat, double lon)
    {
        SetLocation(lat, lon);
    }

    private void SetDiscoverSex(Sex sex)
    {
        if (!(((sex & Sex.Male) == Sex.Male) || ((sex & Sex.Female) == Sex.Female)))
        {
            throw new InvalidUserSexException();
        }
        if ((int) sex > (int) Sex.Female + (int) Sex.Male)
        {
            throw new InvalidUserSexException();
        }
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

    private void SetLocation(double lat, double lon)
    {
        if (lat > 90 | lat < -90 |
            lon > 180 | lon < -180)
        {
            throw new InvalidLocationException();
        }
        Lat = lat;
        Lon = lon;
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