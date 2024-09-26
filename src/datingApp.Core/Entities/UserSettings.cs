using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class UserSettings
{
    public Guid UserId { get; private set; }
    public PreferredSex PreferredSex { get; private set; }
    public int DiscoverAgeFrom { get; private set; }
    public int DiscoverAgeTo { get; private set; }
    public int DiscoverRange { get; private set; }
    public double Lat { get; private set; }
    public double Lon { get; private set; }

    public UserSettings(Guid userId, PreferredSex preferredSex, int discoverAgeFrom, int discoverAgeTo, int discoverRange, double lat, double lon)
    {
        UserId = userId;
        SetPreferredSex(preferredSex);
        SetDiscoverAge(discoverAgeFrom, discoverAgeTo);
        SetDiscoverRange(discoverRange);
        SetLocation(lat, lon);
    }

    public void ChangePreferredSex(PreferredSex sex)
    {
        SetPreferredSex(sex);
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

    private void SetPreferredSex(PreferredSex sex)
    {
        if (!Enum.IsDefined(typeof(PreferredSex), sex))
        {
            throw new InvalidUserDiscoverySexException();
        }
        if (PreferredSex == sex) return;
        PreferredSex = sex;
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