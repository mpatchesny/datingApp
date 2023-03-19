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

        [Fact]
        public void user_settings_age_range_should_be_below_18()
        {
            var location = new Location(36.5, 36.5);
            var ageRange = new Tuple<int, int>(17, 20);
            var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, ageRange, 20, location));
            Assert.NotNull(exception);
        }

        [Fact]
        public void user_settings_age_range_should_not_be_above_100()
        {
            var location = new Location(36.5, 36.5);
            var ageRange = new Tuple<int, int>(18, 101);
            var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, ageRange, 20, location));
            Assert.NotNull(exception);
        }

        [Fact]
        public void user_settings_age_range_first_value_should_be_lower_or_equal_to_second_value()
        {
            var location = new Location(36.5, 36.5);
            var ageRange = new Tuple<int, int>(44, 43);
            var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, ageRange, 20, location));
            Assert.NotNull(exception);
        }

        [Fact]
        public void user_settings_discovery_range_should_not_be_above_100()
        {
            var location = new Location(36.5, 36.5);
            var ageRange = new Tuple<int, int>(20, 25);
            var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, ageRange, 101, location));
            Assert.NotNull(exception);
        }

        [Fact]
        public void user_settings_discovery_range_should_be_above_1()
        {
            var location = new Location(36.5, 36.5);
            var ageRange = new Tuple<int, int>(20, 25);
            var exception = Record.Exception(() =>new UserSettings(1, Sex.Male, ageRange, 0, location));
            Assert.NotNull(exception);
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