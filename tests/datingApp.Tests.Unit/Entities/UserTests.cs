using System;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;

namespace datingApp.Tests.Unit;

public class UserTests
{
    [Fact]
    public void user_phone_should_not_be_emptystring()
    {
        var exception = Record.Exception(() =>new User(1, "", "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }
    
    [Fact]
    public void user_phone_should_not_be_longer_than_9_chars()
    {
        var exception = Record.Exception(() =>new User(1, "0123456789", "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }

    [Theory]
    [InlineData("000+000-0")]
    [InlineData("000#000.0")]
    [InlineData("000a000 0")]
    [InlineData("000a000=0")]
    public void user_phone_should_not_containt_not_numbers(string phone)
    {
        var exception = Record.Exception(() =>new User(1, phone, "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhoneException>(exception);
    }

    [Theory]
    [InlineData("@example.com")]
    [InlineData("user.com")]
    [InlineData("user@@example.com")]
    // [InlineData("user@exam_ple.com")]
    // [InlineData("user@ex ample.com")]
    public void user_email_should_be_proper_email(string badEmail)
    {
        var exception = Record.Exception(() =>new User(1, "012345678", badEmail, "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Fact]
    public void user_email_should_not_exceed_257_chars()
    {
        string badEmail = "";
        for (int i=1; i<=257; i++)
        {
            badEmail += "a";
        }
        badEmail += "test@gmail.com";
        var exception = Record.Exception(() =>new User(1, "012345678", badEmail, "janusz", new System.DateOnly(1999,1,1), Sex.Male,null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Fact]
    public void user_email_should_not_be_emptystring()
    {
        var exception = Record.Exception(() =>new User(1, "012345678", "", "janusz", new System.DateOnly(1999,1,1), Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Fact]
    public void user_name_should_not_be_emptystring()
    {
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "", new System.DateOnly(1999,1,1), Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("123")]
    [InlineData(".")]
    [InlineData(",")]
    [InlineData("#")]
    public void user_name_should_not_contain_invalid_chars(string username)
    {
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Fact]
    public void user_name_should_not_exceed_15_chars()
    {
        string username = "Janusz ma kota a";
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Theory]
    [InlineData("2020-02-02")]
    [InlineData("2005-12-31")]
    [InlineData("1922-01-18")]
    public void user_age_should_be_between_18_and_100(string dateString)
    {
        var invalidDob = DateTime.Parse(dateString);
        var invalidDob2 = new DateOnly(invalidDob.Year, invalidDob.Month, invalidDob.Day);
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "janusz", invalidDob2, Sex.Male, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDateOfBirthException>(exception);
    }

    [Fact]
    public void user_bio_should_not_exceed_400_chars()
    {
        string bio = "";
        for (int i=1; i<=401; i++)
        {
            bio += "a";
        }
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, null, null, null, "", bio));
        Assert.NotNull(exception);
        Assert.IsType<BioTooLongException>(exception);
    }

    [Fact]
    public void user_job_should_not_exceed_30_chars()
    {
        string job = "";
        for (int i=1; i<=31; i++)
        {
            job += "a";
        }
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, null, null, null, job));
        Assert.NotNull(exception);
        Assert.IsType<JobTooLongException>(exception);
    }

    [Fact]
    public void user_get_age_returns_proper_age()
    {
        var actualYear = DateTime.UtcNow.Year;
        var dob = new DateOnly(actualYear - 18, 1, 1);
        var user = new User(1, "012345678", "test@test.com", "janusz", dob, Sex.Male, null, null, null, null);
        Assert.Equal(18, user.GetAge());
    }

    [Fact]
    public void user_should_not_accept_two_sexes()
    {
        var sex = Sex.Male & Sex.Female;
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), sex, null, null, null, null));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUserSexException>(exception);
    }

    [Fact]
    public void change_user_date_of_birth_should_take_effect()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, null, null, null);
        user.ChangeDateOfBirth(new DateOnly(1999,1,2));
        Assert.Equal(new DateOnly(1999,1,2), user.DateOfBirth);
    }

    [Fact]
    public void change_user_bio_should_take_effect()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, null, null, null, "", "bio");
        user.ChangeBio("new bio");
        Assert.Equal("new bio", user.Bio);
    }

    [Fact]
    public void change_user_job_should_take_effect()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, null, null, null, "job");
        user.ChangeJob("new job");
        Assert.Equal("new job", user.Job);
    }
}