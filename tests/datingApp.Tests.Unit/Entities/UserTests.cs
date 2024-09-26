using System;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using Xunit;

namespace datingApp.Tests.Unit;

public class UserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void empty_user_phone_should_throw_InvalidPhoneException(string phone)
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), phone, "email@email.com", "janusz", new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }

    [Fact]
    public void empty_user_phone_should_throw_InvalidPhoneException2()
    {
        string phone = new string(' ', 0);
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), phone, "email@email.com", "janusz", new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }
    
    [Fact]
    public void user_phone_longer_than_9_chars_should_throw_InvalidPhoneException()
    {
        string phone = new string('1', 10);
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), phone, "email@email.com", "janusz", new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }

    [Theory]
    [InlineData("000+000-0")]
    [InlineData("000#000.0")]
    [InlineData("000a000 0")]
    [InlineData("000a000=0")]
    public void user_phone_with_non_numeric_characters_should_throw_InvalidPhoneException(string phone)
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), phone, "email@email.com", "janusz", new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }

    [Theory]
    [InlineData("@example.com")]
    [InlineData("user.com")]
    [InlineData("user@@example.com")]
    // [InlineData("user@exam_ple.com")]
    // [InlineData("user@ex ample.com")]
    public void invalid_user_email_should_throw_InvalidEmailExceptions(string badEmail)
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", badEmail, "janusz", new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Fact]
    public void user_email_longer_than_256_chars_should_throw_InvalidEmailException()
    {
        string badEmail = new string('a', 250);
        badEmail += "@test.com";
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", badEmail, "janusz", new System.DateOnly(1999,1,1), UserSex.Male,null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void empty_or_null_user_email_should_throw_InvalidEmailException(string email)
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", email, "janusz", new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Fact]
    public void empty_or_null_user_email_should_throw_InvalidEmailException2()
    {
        string email = new string('a', 0);
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", email, "janusz", new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void empty_or_null_user_name_should_throw_InvalidUsernameException(string username)
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Fact]
    public void empty_or_null_user_name_should_throw_InvalidUsernameException2()
    {
        string username = new string('a', 0);
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("123")]
    [InlineData(".")]
    [InlineData(",")]
    [InlineData("#")]
    public void user_name_should_with_invalid_chars_should_throw_InvalidUsernameException(string username)
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Fact]
    public void user_name_above_15_chars_should_throw_InvalidUsernameException()
    {
        string username = new string('a', 16);
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    public static TheoryData<DateTime> BadUserAgeData => new()
    {
        { new DateTime(DateTime.Now.Year - 18, DateTime.Now.Month, DateTime.Now.Day + 1)},
        { new DateTime(DateTime.Now.Year - 101, DateTime.Now.Month, DateTime.Now.Day)},
    };

    [Theory]
    [MemberData(nameof(BadUserAgeData))]
    public void user_age_below_18_or_above_100_should_throw_InvalidDateOfBirthException(DateTime date)
    {
        var invalidDob = new DateOnly(date.Year, date.Month, date.Day);
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", invalidDob, UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDateOfBirthException>(exception);
    }

    [Fact]
    public void empty_date_of_birth_should_throw_DateOfBirthCannotBeEmptyException()
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(), UserSex.Male, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<DateOfBirthCannotBeEmptyException>(exception);
    }

    [Fact]
    public void user_bio_longer_than_400_chars_should_throw_BioTooLongException()
    {
        string bio = new string('a', 401);
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings, "", bio));
        Assert.NotNull(exception);
        Assert.IsType<BioTooLongException>(exception);
    }

    [Fact]
    public void user_job_longer_than_50_chars_should_throw_JobTooLongException()
    {
        string job = new string('a', 51);
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings, job));
        Assert.NotNull(exception);
        Assert.IsType<JobTooLongException>(exception);
    }

    [Fact]
    public void user_get_age_returns_proper_age()
    {
        var actualYear = DateTime.UtcNow.Year;
        var dob = new DateOnly(actualYear - 18, 1, 1);
        var user = new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", dob, UserSex.Male, null, _properUserSettings);
        Assert.Equal(18, user.GetAge());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(0)]
    [InlineData(7)]
    public void invalid_user_sex_should_throw_InvalidUserSexException(int sex)
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), (UserSex) sex, null, _properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUserSexException>(exception);
    }

    [Fact]
    public void change_user_date_of_birth_should_take_effect()
    {
        var user = new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings);
        user.ChangeDateOfBirth(new DateOnly(1999,1,2));
        Assert.Equal(new DateOnly(1999,1,2), user.DateOfBirth);
    }

    [Fact]
    public void change_user_bio_should_take_effect()
    {
        var user = new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings, "", "bio");
        user.ChangeBio("new bio");
        Assert.Equal("new bio", user.Bio);
    }

    [Fact]
    public void change_user_job_should_take_effect()
    {
        var user = new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, null, _properUserSettings, "job");
        user.ChangeJob("new job");
        Assert.Equal("new job", user.Job);
    }

    [Fact]
    public void null_user_settings_should_throw_UserSettingsIsNullException()
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Female, null, null));
        Assert.NotNull(exception);
        Assert.IsType<UserSettingsIsNullException>(exception);
    }

    [Fact]
    public void create_new_user_with_empty_bio_and_job_should_succeed()
    {
        var exception = Record.Exception(() =>new User(Guid.NewGuid(), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Female, null, _properUserSettings, "", ""));
        Assert.Null(exception);
    }

    private readonly UserSettings _properUserSettings;
    public UserTests()
    {
        _properUserSettings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, 18, 20, 20, 45.5, 45.5);
    }
}