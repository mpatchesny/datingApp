using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;

namespace datingApp.Tests.Unit.Entities;

public class UserSettingsTests
{
    [Fact]
    public void user_settings_should_accept_male_female_discovery_sex()
    {
        var discoverySex = Sex.Male & Sex.Female;
        var exception = Record.Exception(() =>new UserSettings(1, discoverySex, new AgeRange(20, 21), 20));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(17, 18)]
    [InlineData(18, 101)]
    [InlineData(21, 20)]
    public void user_settings_age_range_should_be_between_18_and_100_and_min_not_larger_than_max(int minAge, int maxAge)
    {
        var exception = Record.Exception(() =>new AgeRange(minAge, maxAge));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryAgeException>(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void user_settings_discovery_range_should_be_between_1_and_100(int range)
    {
        var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, new AgeRange(20, 25), range));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDiscoveryRangeException>(exception);
    }
}