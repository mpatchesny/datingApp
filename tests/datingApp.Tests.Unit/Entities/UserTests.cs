using System;
using datingApp.Core.Entities;

namespace datingApp.Tests.Unit;

public class UserTests
{
    [Fact]
    public void user_phone_should_not_be_emptystring()
    {
        var exception = Record.Exception(() =>new User(1, "", "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
    }
    [Fact]
    public void user_phone_should_not_be_longer_than_9_chars()
    {
        var exception = Record.Exception(() =>new User(1, "0123456789", "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData("000+000-0")]
    [InlineData("000#000.0")]
    [InlineData("000a000 0")]
    [InlineData("000a000=0")]
    public void user_phone_should_not_containt_not_numbers(string phone)
    {
        var exception = Record.Exception(() =>new User(1, phone, "email@email.com", "janusz", new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData("user@example")]
    [InlineData("user.com")]
    [InlineData("user@exam_ple.com")]
    [InlineData("user@ex ample.com")]
    [InlineData("@example.com")]
    public void user_email_should_be_proper_email(string email)
    {
        var exception = Record.Exception(() =>new User(1, "012345678", email, "janusz", new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_email_should_not_exceed_320_chars()
    {
        string email = "";
        for (int i=1; i<=320; i++)
        {
            email += "a";
        }
        email += "test@gmail.com";
        var exception = Record.Exception(() =>new User(1, "012345678", email, "janusz", new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_email_should_not_be_emptystring()
    {
        var exception = Record.Exception(() =>new User(1, "012345678", "", "janusz", new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_name_should_not_be_emptystring()
    {
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "", new System.DateOnly(1999,1,1), Sex.Male));
    }

    [Theory]
    [InlineData("@")]
    [InlineData("123")]
    [InlineData(".")]
    [InlineData(",")]
    [InlineData("#")]
    public void user_name_should_not_contain_invalid_chars(string username)
    {
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_name_should_not_exceed_15_chars()
    {
        string username = "Janusz ma kota a";
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData("2020-02-02")]
    [InlineData("2005-12-31")]
    [InlineData("1922-01-18")]
    public void user_age_should_be_between_18_and_100(string dateString)
    {
        var invalidDob = DateTime.Parse(dateString);
        var invalidDob2 = new DateOnly(invalidDob.Year, invalidDob.Month, invalidDob.Day);
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "janusz", invalidDob2, Sex.Male));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_bio_should_not_exceed_400_chars()
    {
        string bio = "";
        for (int i=1; i<=401; i++)
        {
            bio += "a";
        }
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, "", bio));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_job_should_not_exceed_30_chars()
    {
        string job = "";
        for (int i=1; i<=31; i++)
        {
            job += "a";
        }
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, job));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_get_age_returns_proper_age()
    {
        var actualYear = DateTime.UtcNow.Year;
        var dob = new DateOnly(actualYear - 18, 1, 1);
        var user = new User(1, "012345678", "test@test.com", "janusz", dob, Sex.Male);
        Assert.Equal(18, user.GetAge());
    }
}