using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class UserLocation
{
    public int UserId { get; private set; }
    public Location Location { get; private set; }

    public UserLocation(int userId, Location location)
    {
        UserId = userId;
        SetLocation(location);
    }
    public void ChangeLocation(Location location)
    {
        SetLocation(location);
    }
    
    private void SetLocation(Location location)
    {
        if (Location == location) return;
        Location = location;
    }
}