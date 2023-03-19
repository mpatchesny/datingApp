using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;

namespace datingApp.Tests.Unit.Entities
{
    public class UserSettingsTests
    {
        [Fact]
        public void user_settings_should_accept_male_female_discovery_sex()
        {
            var location = new Location(36.5, 36.5);
            var ageRange = new Tuple<int, int>(18, 20);
            var discoverySex = Sex.Male & Sex.Female;
            var exception = Record.Exception(() =>new UserSettings(1, discoverySex, ageRange, 20, location));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(17, 18)]
        [InlineData(18, 101)]
        [InlineData(21, 20)]
        public void user_settings_age_range_should_be_between_18_and_100_and_min_not_larger_than_max(int minAge, int maxAge)
        {
            var location = new Location(36.5, 36.5);
            var ageRange = new Tuple<int, int>(minAge, maxAge);
            var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, ageRange, 20, location));
            Assert.NotNull(exception);
            Assert.IsType<InvalidDiscoveryAgeException>(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void user_settings_discovery_range_should_be_between_1_and_100(int range)
        {
            var location = new Location(36.5, 36.5);
            var ageRange = new Tuple<int, int>(20, 25);
            var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, ageRange, range, location));
            Assert.NotNull(exception);
            Assert.IsType<InvalidDiscoveryRangeException>(exception);
        }

        [Theory]
        [InlineData(0.0, -180.1)]
        [InlineData(0.0, 180.1)]
        [InlineData(90.1, 0.0)]
        [InlineData(-90.1, 0.0)]
        public void user_settings_location_should_be_proper_location(double lat, double lon)
        {
            var exception = Record.Exception(() => new Location(lat, lon));
            Assert.NotNull(exception);
            Assert.IsType<InvalidLocationException>(exception);
        }
    }
}