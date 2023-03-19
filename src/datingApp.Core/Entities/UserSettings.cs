using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class UserSettings
{
    public int UserId { get; private set; }
    public Sex DiscoverSex { get; private set; }
    public Tuple<int, int> DiscoverAgeRange { get; private set; }
    public int DiscoverRange { get; private set; }
    public Location Location { get; private set; }

    public UserSettings(int userId, Sex discoverSex, Tuple<int,int> discoverAgeRange, int discoverRange, Location location)
    {
        UserId = userId;
        DiscoverSex = discoverSex;
        SetDiscoverAge(discoverAgeRange);
        SetDiscoverRange(discoverRange);
        SetLocation(location);
    }

    public void ChangeDiscoverSex(Sex sex)
    {
        DiscoverSex = sex;
    }

    public void ChangeDiscoverAge(Tuple<int,int> discoverAgeRange)
    {
        SetDiscoverAge(discoverAgeRange);
    }

    public void ChangeDiscoverRange(int discoverRange)
    {
        SetDiscoverRange(discoverRange);
    }

    public void ChangeLocation(Location location)
    {
        SetLocation(location);
    }

    private void SetDiscoverAge(Tuple<int, int> discoverAgeRange)
    {
        if (discoverAgeRange.Item1 < 18 | discoverAgeRange.Item1 > 100 |
            discoverAgeRange.Item2 < 18 | discoverAgeRange.Item2 > 100)
        {
            throw new Exception("discover max age must be between 18 and 100");
        }
        if (discoverAgeRange.Item1 > discoverAgeRange.Item2)
        {
            throw new Exception("discover min age must be lower or equal to max age");
        }
        if (DiscoverAgeRange == discoverAgeRange) return;
        DiscoverAgeRange = discoverAgeRange;
    }

    private void SetDiscoverRange(int discoverRange)
    {
        if (discoverRange < 1 | discoverRange > 100)
        {
            throw new Exception("discover range must be between 1 and 100");
        }
        if (DiscoverRange == discoverRange) return;
        DiscoverRange = discoverRange;
    }

    private void SetLocation(Location location)
    {
        if (location.Lat > 90 | location.Lat < -90 |
            location.Lon > 180 | location.Lon < -180)
        {
            throw new Exception($"invalid location {location.ToString()}");
        }
        if (Location == location) return;
        Location = location;
    }
}