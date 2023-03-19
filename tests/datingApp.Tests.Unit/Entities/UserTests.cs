using datingApp.Core.Entities;

namespace datingApp.Tests.Unit;

public class UsersTests
{
    [Fact]
    public void user_phone_has_valid_length()
    {
        var user = new User(1, "123456789123456789", "xddd", "janusz", new System.DateOnly(1999,1,1), Sex.Male, "nmo job", "no bio");
    }

    public void user_phone_has_only_numbers()
    {
    }

    public void user_email_is_proper_email()
    {
    }

    public void user_email_should_not_exceed_320_chars()
    {
    }

    public void user_name_is_not_null()
    {
    }

    public void user_name_does_not_contain_invalid_chars()
    {
    }

    public void user_name_should_not_exceed_15_chars()
    {
    }

    public void user_age_should_be_between_18_and_100()
    {
    }

    public void user_bio_should_not_exceed_400_chars()
    {
    }

    public void user_job_should_not_exceed_30_chars()
    {
    }

    public void user_get_age_returns_proper_age()
    {
    }
}