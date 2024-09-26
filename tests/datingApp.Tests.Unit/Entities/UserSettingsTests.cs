using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class UserSettingsTests
{
    [Fact]
    public void user_settings_should_accept_male_female_discovery_sex()
    {
        var discoverySex = PreferredSex.Male | PreferredSex.Female;
        var exception = Record.Exception(() =>new UserSettings(Guid.NewGuid(), discoverySex, 20, 21, 20, 45.5, 45.5));
        Assert.Null(exception);
    }

    [Fact]
    public void user_settings_should_accept_male_discovery_sex()
    {
        var discoverySex = PreferredSex.Male;
        var exception = Record.Exception(() =>new UserSettings(Guid.NewGuid(), discoverySex, 20, 21, 20, 45.5, 45.5));
        Assert.Null(exception);
    }

    [Fact]
    public void user_settings_should_accept_female_discovery_sex()
    {
        var discoverySex = PreferredSex.Female;
        var exception = Record.Exception(() =>new UserSettings(Guid.NewGuid(), discoverySex, 20, 21, 20, 45.5, 45.5));
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
    public void invalid_user_settings_discovery_sex_should_throw_InvalidUserDiscoverySexException(int discoverySex)
    {
        var exception = Record.Exception(() =>new UserSettings(Guid.NewGuid(), (PreferredSex) discoverySex, 20, 21, 20, 45.5, 45.5));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUserDiscoverySexException>(exception);
    }

    [Theory]
    [InlineData(17, 18)]
    [InlineData(18, 101)]
    public void user_settings_age_range_below_18_or_above_100_should_throw_InvalidDiscoveryAgeException(int minAge, int maxAge)
    {
        var exception = Record.Exception(() =>new UserSettings(Guid.NewGuid(), PreferredSex.Male, minAge, maxAge, 20, 45.5, 45.5));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryAgeException>(exception);
    }
    [Theory]
    [InlineData(21, 20)]
    public void user_settings_age_range_with_age_to_below_age_from_should_throw_InvalidDiscoveryAgeException(int minAge, int maxAge)
    {
        var exception = Record.Exception(() =>new UserSettings(Guid.NewGuid(), PreferredSex.Male, minAge, maxAge, 20, 45.5, 45.5));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryAgeException>(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void user_settings_discovery_range_below_1_or_above_100_should_throw_InvalidDiscoveryRangeException(int range)
    {
        var exception = Record.Exception(() =>new UserSettings(Guid.NewGuid(), PreferredSex.Male, 20, 25, range, 45.5, 45.5));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryRangeException>(exception);
    }

    [Theory]
    [InlineData(0.0, -180.1)]
    [InlineData(0.0, 180.1)]
    [InlineData(90.1, 0.0)]
    [InlineData(-90.1, 0.0)]
    public void user_settings_invalid_location_should_throw_InvalidLocationException(double lat, double lon)
    {
        var exception = Record.Exception(() =>new UserSettings(Guid.NewGuid(), PreferredSex.Male, 20, 25, 20, lat, lon));
        Assert.NotNull(exception);
        Assert.IsType<InvalidLocationException>(exception);
    }

    [Fact]
    public void user_settings_location_change_should_take_effect()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, 20, 25, 20, 40.5, 40.5);
        settings.ChangeLocation(45.5, 46.5);
        Assert.Equal(45.5, settings.Lat);
        Assert.Equal(46.5, settings.Lon);
    }

    [Fact]
    public void user_settings_discovery_age_range_change_should_take_effect()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, 20, 25, 20, 40.5, 40.5);
        settings.ChangePreferredAge(18, 60);
        Assert.Equal(18, settings.PreferredAgeFrom);
        Assert.Equal(60, settings.PreferredAgeTo);
    }

    [Fact]
    public void user_settings_change_range_should_take_effect()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, 20, 25, 20, 40.5, 40.5);
        settings.ChangeDiscoverRange(40);
        Assert.Equal(40, settings.DiscoverRange);
    }
}