using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;
using Microsoft.AspNetCore.Routing.Constraints;

namespace datingApp.Core.Entities;

public class UserSettings
{
    public UserId UserId { get; private set; }
    public PreferredSex PreferredSex { get; private set; }
    public PreferredAge PreferredAge { get; private set; }
    public PreferredMaxDistance PreferredMaxDistance { get; private set; }
    public Location Location { get; private set; }

    private UserSettings()
    {
        // EF
    }

    public UserSettings(UserId userId, PreferredSex preferredSex, PreferredAge preferredAge, PreferredMaxDistance preferredMaxDistance, Location location)
    {
        UserId = userId;
        SetPreferredSex(preferredSex);
        PreferredAge = preferredAge;
        PreferredMaxDistance = preferredMaxDistance;
        Location = location;
    }

    public void ChangePreferredSex(PreferredSex sex)
    {
        SetPreferredSex(sex);
    }

    public void ChangePreferredAge(PreferredAge preferredAge)
    {
        PreferredAge = preferredAge;
    }

    public void ChangePreferredMaxDistance(PreferredMaxDistance preferredMaxDistance)
    {
        PreferredMaxDistance = preferredMaxDistance;
    }

    public void ChangeLocation(Location location)
    {
        Location = location;
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

    public override bool Equals(object obj)
    {
        if (obj is not UserSettings) return false;
        UserSettings userSettings = (UserSettings) obj;
        return userSettings.UserId == UserId && 
            userSettings.PreferredSex == PreferredSex &&
            userSettings.PreferredAge == PreferredAge &&
            userSettings.PreferredMaxDistance == PreferredMaxDistance &&
            userSettings.Location == Location;
    }
}