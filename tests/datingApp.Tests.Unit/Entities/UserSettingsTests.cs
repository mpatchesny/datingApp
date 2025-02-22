using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class UserSettingsTests
{
    [Fact]
    public void user_settings_should_accept_male_female_discovery_sex()
    {
        var discoverySex = PreferredSex.Male | PreferredSex.Female;
        var exception = Record.Exception(() => new UserSettings(Guid.NewGuid(), discoverySex, new PreferredAge(20, 21), 20, new Location(45.5, 45.5)));
        Assert.Null(exception);
    }

    [Fact]
    public void user_settings_should_accept_male_discovery_sex()
    {
        var discoverySex = PreferredSex.Male;
        var exception = Record.Exception(() => new UserSettings(Guid.NewGuid(), discoverySex, new PreferredAge(20, 21), 20, new Location(45.5, 45.5)));
        Assert.Null(exception);
    }

    [Fact]
    public void user_settings_should_accept_female_discovery_sex()
    {
        var discoverySex = PreferredSex.Female;
        var exception = Record.Exception(() => new UserSettings(Guid.NewGuid(), discoverySex, new PreferredAge(20, 21), 20, new Location(45.5, 45.5)));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void invalid_user_settings_discovery_sex_throws_InvalidUserDiscoverySexException(int discoverySex)
    {
        var exception = Record.Exception(() => new UserSettings(Guid.NewGuid(), (PreferredSex) discoverySex, new PreferredAge(20, 21), 20, new Location(45.5, 45.5)));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUserDiscoverySexException>(exception);
    }

    [Theory]
    [InlineData(17, 18)]
    [InlineData(18, 101)]
    public void user_settings_age_range_below_18_or_above_100_throws_InvalidDiscoveryAgeException(int minAge, int maxAge)
    {
        var exception = Record.Exception(() => new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(minAge, maxAge), 20, new Location(45.5, 45.5)));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryAgeException>(exception);
    }
    [Theory]
    [InlineData(21, 20)]
    public void user_settings_age_range_with_age_to_lower_than_age_from_throws_InvalidDiscoveryAgeException(int minAge, int maxAge)
    {
        var exception = Record.Exception(() => new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(minAge, maxAge), 20, new Location(45.5, 45.5)));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryAgeException>(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void user_settings_discovery_range_below_1_or_above_100_throws_InvalidDiscoveryRangeException(int range)
    {
        var exception = Record.Exception(() => new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(20, 25), range, new Location(45.5, 45.5)));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryRangeException>(exception);
    }

    [Theory]
    [InlineData(0.0, -180.1)]
    [InlineData(0.0, 180.1)]
    [InlineData(90.1, 0.0)]
    [InlineData(-90.1, 0.0)]
    public void user_settings_invalid_location_throws_InvalidLocationException(double lat, double lon)
    {
        var exception = Record.Exception(() => new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(20, 25), 20, new Location(lat, lon)));
        Assert.NotNull(exception);
        Assert.IsType<InvalidLocationException>(exception);
    }

    [Fact]
    public void user_settings_ChangeLocation_should_change_location()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(20, 25), 20, new Location(40.5, 40.5));
        settings.ChangeLocation(new Location(45.5, 46.5));
        Assert.Equal(45.5, settings.Location.Lat);
        Assert.Equal(46.5, settings.Location.Lon);
    }

    [Fact]
    public void user_settings_ChangePreferredAge_should_change_preferred_age()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(20, 25), 20, new Location(40.5, 40.5));
        settings.ChangePreferredAge(new PreferredAge(18, 60));
        Assert.Equal(18, settings.PreferredAge.From);
        Assert.Equal(60, settings.PreferredAge.To);
    }

    [Fact]
    public void user_settings_ChangePreferredMaxDistance_should_change_preferred_max_distance()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(20, 25), 20, new Location(40.5, 40.5));
        settings.ChangePreferredMaxDistance(40);
        Assert.Equal(40, settings.PreferredMaxDistance.Value);
    }
}