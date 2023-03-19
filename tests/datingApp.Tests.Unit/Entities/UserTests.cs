using System;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;

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
    [InlineData("@example.com")]
    [InlineData("user.com")]
    [InlineData("user@@example.com")]
    // [InlineData("user@exam_ple.com")]
    // [InlineData("user@ex ample.com")]
    public void user_email_should_be_proper_email(string badEmail)
    {
        var exception = Record.Exception(() =>new User(1, "012345678", badEmail, "janusz", new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
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
        var exception = Record.Exception(() =>new User(1, "012345678", badEmail, "janusz", new System.DateOnly(1999,1,1), Sex.Male));
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
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male));
        Assert.NotNull(exception);
        Assert.IsType<InvalidUsernameException>(exception);
    }

    [Fact]
    public void user_name_should_not_exceed_15_chars()
    {
        string username = "Janusz ma kota a";
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", username, new System.DateOnly(1999,1,1), Sex.Male));
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

    [Fact]
    public void user_should_not_accept_two_sexes()
    {
        var sex = Sex.Male & Sex.Female;
        var exception = Record.Exception(() =>new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), sex));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_adding_two_matches_with_same_id_should_fail()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male);
        var match1 = new Match(1, 1, 1, null, DateTime.UtcNow);
        var match2 = new Match(1, 1, 1, null, DateTime.UtcNow);
        user.AddMatch(match1);
        var exception = Record.Exception(() => user.AddMatch(match2));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_adding_two_photos_with_same_id_should_fail()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male);
        var photo1 = new Photo(1, 1, "abc", 1);
        var photo2 = new Photo(1, 1, "abc", 2);
        user.AddPhoto(photo1);
        var exception = Record.Exception(() => user.AddPhoto(photo2));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_should_not_accept_two_photos_with_same_oridinal()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male);
        var photo1 = new Photo(1, 1, "abc", 1);
        var photo2 = new Photo(2, 1, "abc", 1);
        user.AddPhoto(photo1);
        var exception = Record.Exception(() => user.AddPhoto(photo2));
        Assert.NotNull(exception);
    }

    [Fact]
    public void user_with_main_photo_isvisible_should_return_true()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male);
        var photo1 = new Photo(1, 1, "abc", 1);
        user.AddPhoto(photo1);
        Assert.Equal(true, user.IsVisible());
    }

    [Fact]
    public void change_user_date_of_birth_should_take_effect()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male);
        user.ChangeDateOfBirth(new DateOnly(1999,1,2));
        Assert.Equal(new DateOnly(1999,1,2), user.DateOfBirth);
    }

    [Fact]
    public void change_user_bio_should_take_effect()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, "", "bio");
        user.ChangeBio("new bio");
        Assert.Equal("new bio", user.Bio);
    }

    [Fact]
    public void change_user_job_should_take_effect()
    {
        var user = new User(1, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, "job");
        user.ChangeJob("new job");
        Assert.Equal("new job", user.Job);
    }
}