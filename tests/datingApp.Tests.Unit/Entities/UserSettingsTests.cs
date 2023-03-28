using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        var discoverySex = Sex.Male & Sex.Female;
        var exception = Record.Exception(() =>new UserSettings(1, discoverySex, 20, 21, 20, 45.5, 45.5));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(17, 18)]
    [InlineData(18, 101)]
    [InlineData(21, 20)]
    public void user_settings_age_range_should_be_between_18_and_100_and_min_not_larger_than_max(int minAge, int maxAge)
    {
        var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, minAge, maxAge, 20, 45.5, 45.5));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryAgeException>(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void user_settings_discovery_range_should_be_between_1_and_100(int range)
    {
        var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, 20, 25, range, 45.5, 45.5));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryRangeException>(exception);
    }

    [Theory]
    [InlineData(0.0, -180.1)]
    [InlineData(0.0, 180.1)]
    [InlineData(90.1, 0.0)]
    [InlineData(-90.1, 0.0)]
    public void user_location_should_be_proper_location(double lat, double lon)
    {
        var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, 20, 25, 20, lat, lon));
        Assert.NotNull(exception);
        Assert.IsType<InvalidLocationException>(exception);
    }

    [Fact]
    public void user_settings_location_change_should_take_effect()
    {
        var settings = new UserSettings(1, Sex.Male, 20, 25, 20, 40.5, 40.5);
        settings.ChangeLocation(45.5, 46.5);
        Assert.Equal(settings.Lat, 45.5);
        Assert.Equal(settings.Lon, 46.5);
    }

    [Fact]
    public void user_settings_discovery_age_range_change_should_take_effect()
    {
        var settings = new UserSettings(1, Sex.Male, 20, 25, 20, 40.5, 40.5);
        settings.ChangeDiscoverAge(18, 60);
        Assert.Equal(settings.DiscoverAgeFrom, 18);
        Assert.Equal(settings.DiscoverAgeTo, 60);
    }

    [Fact]
    public void user_settings_change_range_should_take_effect()
    {
        var settings = new UserSettings(1, Sex.Male, 20, 25, 20, 40.5, 40.5);
        settings.ChangeDiscoverRange(40);
        Assert.Equal(settings.DiscoverRange, 40);
    }
}