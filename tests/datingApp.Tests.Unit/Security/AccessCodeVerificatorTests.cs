using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using datingApp.Infrastructure.Security;
using Xunit;

namespace datingApp.Tests.Unit.Security;

public class AccessCodeVerificatorTests
{
    [Theory]
    [InlineData("0123456798")]
    [InlineData("test@test.com")]
    public void given_valid_email_or_phone_code_should_be_verified_positive(string emailOrPhone)
    {
        var accessCode = "1234";
        var code = new AccessCodeDto
        {
            AccessCode = accessCode,
            EmailOrPhone = emailOrPhone,
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromDays(1),
            Expiry = TimeSpan.FromDays(1)
        };

        var verificator = new AccessCodeVerificator();
        Assert.True(verificator.Verify(code, accessCode, emailOrPhone));
    }

    [Theory]
    [InlineData("0123456798", "123456798")]
    [InlineData("test@test.com", "test1@test.com")]
    public void given_invalid_email_or_phone_code_should_be_verified_negative(string goodEmailOrPhone, string badEmailOrPhone)
    {
        var accessCode = "1234";
        var code = new AccessCodeDto
        {
            AccessCode = accessCode,
            EmailOrPhone = goodEmailOrPhone,
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromDays(1),
            Expiry = TimeSpan.FromDays(1)
        };

        var verificator = new AccessCodeVerificator();
        Assert.False(verificator.Verify(code, accessCode, badEmailOrPhone));
    }

    [Theory]
    [InlineData("TEst@test.coM", "tESt@tesT.COm")]
    [InlineData("TEST@TEST.COM", "test@test.com")]
    [InlineData("test@test.com", "TEST@TEST.COM")]
    [InlineData("TEST@TEST.COM", "TEST@TEST.COM")]
    public void given_valid_email_in_different_case_code_should_be_verified_positive(string email, string differentCaseEmail)
    {
        var accessCode = "1234";
        var code = new AccessCodeDto
        {
            AccessCode = accessCode,
            EmailOrPhone = email,
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromDays(1),
            Expiry = TimeSpan.FromDays(1)
        };

        var verificator = new AccessCodeVerificator();
        Assert.True(verificator.Verify(code, accessCode, differentCaseEmail));
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("ABcDe")]
    [InlineData("aaBBccDD")]
    [InlineData("abcde")]
    public void given_valid_code_code_should_be_verified_positive_with_case_sensitive_verification(string accessCode)
    {
        var code = new AccessCodeDto
        {
            AccessCode = accessCode,
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromDays(1),
            Expiry = TimeSpan.FromDays(1)
        };

        var verificator = new AccessCodeVerificator();
        Assert.True(verificator.Verify(code, accessCode, "test@test.com"));
    }

    [Theory]
    [InlineData("123456", "12346")]
    [InlineData("ABCDE", "ABCD")]
    [InlineData("ABCDE", "abcd")]
    public void given_invalid_code_code_should_be_verified_negative_with_case_sensitive_verification(string goodCode, string badCode)
    {
        var code = new AccessCodeDto
        {
            AccessCode = goodCode,
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromDays(1),
            Expiry = TimeSpan.FromDays(1)
        };

        var verificator = new AccessCodeVerificator();
        Assert.False(verificator.Verify(code, badCode, "test@test.com"));
    }

    [Fact]
    public void expired_code_should_be_verified_negative()
    {
        var accessCode = "1234";
        var code = new AccessCodeDto
        {
            AccessCode = accessCode,
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow - TimeSpan.FromSeconds(1),
            Expiry = -TimeSpan.FromSeconds(1)
        };

        var verificator = new AccessCodeVerificator();
        Assert.False(verificator.Verify(code, accessCode, "test@test.com"));
    }

    [Fact]
    public void nonexpired_code_should_be_verified_positive()
    {
        var accessCode = "1234";
        var code = new AccessCodeDto
        {
            AccessCode = accessCode,
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromSeconds(1),
            Expiry = TimeSpan.FromSeconds(1)
        };

        var verificator = new AccessCodeVerificator();
        Assert.True(verificator.Verify(code, accessCode, "test@test.com"));
    }
}