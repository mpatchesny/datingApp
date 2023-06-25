using System;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using Xunit;

namespace datingApp.Tests.Unit;

public class UserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void empty_user_phone_should_throw_exception(string phone)
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), phone, "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }
    
    [Fact]
    public void user_phone_longer_than_9_chars_should_throw_exception()
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "0123456789", "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }

    [Theory]
    [InlineData("000+000-0")]
    [InlineData("000#000.0")]
    [InlineData("000a000 0")]
    [InlineData("000a000=0")]
    public void user_phone_with_non_numeric_characters_should_throw_exception(string phone)
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), phone, "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }

    [Theory]
    [InlineData("@example.com")]
    [InlineData("user.com")]
    [InlineData("user@@example.com")]
    // [InlineData("user@exam_ple.com")]
    // [InlineData("user@ex ample.com")]
    public void invalid_user_email_should_throw_exception(string badEmail)
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", badEmail, "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Fact]
    public void user_email_longer_than_256_chars_should_throw_exception()
    {
        string badEmail = "";
        for (int i=1; i<=257; i++)
        {
            badEmail += "a";
        }
        badEmail += "test@gmail.com";
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", badEmail, "janusz", new System.DateOnly(1999,1,1), Sex.Male,null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void empty_or_null_user_email_should_throw_exception(string email)
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", email, "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void empty_or_null_user_name_should_throw_exception(string username)
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("123")]
    [InlineData(".")]
    [InlineData(",")]
    [InlineData("#")]
    public void user_name_should_with_invalid_chars_should_throw_exception(string username)
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Fact]
    public void user_name_above_15_chars_should_throw_exception()
    {
        string username = "Janusz ma kota a";
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Theory]
    [InlineData("2020-02-02")]
    [InlineData("2005-12-31")]
    [InlineData("1922-01-18")]
    public void user_age_below_18_or_aboce_100_should_throw_exception(string dateString)
    {
        var invalidDob = DateTime.Parse(dateString);
        var invalidDob2 = new DateOnly(invalidDob.Year, invalidDob.Month, invalidDob.Day);
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", invalidDob2, Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDateOfBirthException>(exception);
    }

    [Fact]
    public void try_set_empty_date_as_date_of_birth_should_throw_DateOfBirthCannotBeEmptyException()
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", new DateOnly(), Sex.Male, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<DateOfBirthCannotBeEmptyException>(exception);
    }

    [Fact]
    public void user_bio_longer_than_400_chars_should_throw_exception()
    {
        string bio = "";
        for (int i=1; i<=401; i++)
        {
            bio += "a";
        }
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, properUserSettings, "", bio));
        Assert.NotNull(exception);
        Assert.IsType<BioTooLongException>(exception);
    }

    [Fact]
    public void user_job_longer_than_50_chars_should_throw_exception()
    {
        string job = "";
        for (int i=1; i<=51; i++)
        {
            job += "a";
        }
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, properUserSettings, job));
        Assert.NotNull(exception);
        Assert.IsType<JobTooLongException>(exception);
    }

    [Fact]
    public void user_get_age_returns_proper_age()
    {
        var actualYear = DateTime.UtcNow.Year;
        var dob = new DateOnly(actualYear - 18, 1, 1);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", dob, Sex.Male, null, properUserSettings);
        Assert.Equal(18, user.GetAge());
    }

    [Theory]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(7)]
    public void invalid_user_sex_should_throw_exception(int sex)
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), (Sex) sex, null, properUserSettings));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUserSexException>(exception);
    }

    [Fact]
    public void change_user_date_of_birth_should_take_effect()
    {
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, properUserSettings);
        user.ChangeDateOfBirth(new DateOnly(1999,1,2));
        Assert.Equal(new DateOnly(1999,1,2), user.DateOfBirth);
    }

    [Fact]
    public void change_user_bio_should_take_effect()
    {
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, properUserSettings, "", "bio");
        user.ChangeBio("new bio");
        Assert.Equal("new bio", user.Bio);
    }

    [Fact]
    public void change_user_job_should_take_effect()
    {
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, properUserSettings, "job");
        user.ChangeJob("new job");
        Assert.Equal("new job", user.Job);
    }

    [Fact]
    public void null_user_settings_should_throw_exception()
    {
        var exception = Record.Exception(() =>new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Female, null, null));
        Assert.NotNull(exception);
        Assert.IsType<UserSettingsIsNullException>(exception);
    }

    private readonly UserSettings properUserSettings;
    public UserTests()
    {
        properUserSettings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 20, 45.5, 45.5);
    }
}