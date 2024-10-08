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
    public int PreferredAgeFrom { get; private set; }
    public int PreferredAgeTo { get; private set; }
    public int PreferredMaxDistance { get; private set; }
    public double Lat { get; private set; }
    public double Lon { get; private set; }

    public UserSettings(Guid userId, PreferredSex preferredSex, int preferredAgeFrom, int preferredAgeTo, int preferredMaxDistance, double lat, double lon)
    {
        UserId = userId;
        SetPreferredSex(preferredSex);
        SetPreferredAge(preferredAgeFrom, preferredAgeTo);
        SetPreferredMaxDistance(preferredMaxDistance);
        SetLocation(lat, lon);
    }

    public void ChangePreferredSex(PreferredSex sex)
    {
        SetPreferredSex(sex);
    }

    public void ChangePreferredAge(int preferredAgeFrom, int preferredAgeTo)
    {
        SetPreferredAge(preferredAgeFrom, preferredAgeTo);
    }

    public void ChangePreferredMaxDistance(int preferredMaxDistance)
    {
        SetPreferredMaxDistance(preferredMaxDistance);
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

    private void SetPreferredAge(int preferredAgeFrom, int preferredAgeTo)
    {
        if (preferredAgeFrom < 18 | preferredAgeFrom > 100)
        {
            throw new InvalidDiscoveryAgeException("minimum discover age must be between 18 and 100");
        }
        else if (preferredAgeTo < 18 | preferredAgeTo > 100)
        {
            throw new InvalidDiscoveryAgeException("maximum discover age must be between 18 and 100");
        }
        else if (preferredAgeFrom > preferredAgeTo)
        {
            throw new InvalidDiscoveryAgeException("minimum discover age cannot be larger than maximum discover age");
        }
        PreferredAgeFrom = preferredAgeFrom;
        PreferredAgeTo = preferredAgeTo;
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

    private void SetPreferredMaxDistance(int discoverRange)
    {
        if (discoverRange < 1 | discoverRange > 100)
        {
            throw new InvalidDiscoveryRangeException();
        }
        if (PreferredMaxDistance == discoverRange) return;
        PreferredMaxDistance = discoverRange;
    }
}