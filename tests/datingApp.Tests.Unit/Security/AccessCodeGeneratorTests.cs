using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.Security;

public class AccessCodeGeneratorTests
{
    [Fact]
    public void generated_code_email_matches_given_email()
    {
        IOptions<AccessCodeOptions> options = Options.Create<AccessCodeOptions>(new AccessCodeOptions());
        options.Value.Expiry = TimeSpan.FromMinutes(15);
        var generator = new AccessCodeGenerator(options);
        string email = "test@test.com";
        var code = generator.GenerateCode(email);
        Assert.Equal(email, code.EmailOrPhone);
    }

    [Fact]
    public void generated_code_is_six_characters_long()
    {
        IOptions<AccessCodeOptions> options = Options.Create<AccessCodeOptions>(new AccessCodeOptions());
        options.Value.Expiry = TimeSpan.FromMinutes(15);
        var generator = new AccessCodeGenerator(options);
        string email = "test@test.com";
        var code = generator.GenerateCode(email);
        Assert.Equal(6, code.AccessCode.Length);
    }

    [Fact]
    public void generated_code_timespan_equals_timespan_from_options()
    {
        IOptions<AccessCodeOptions> options = Options.Create<AccessCodeOptions>(new AccessCodeOptions());
        var timeSpan = TimeSpan.FromMinutes(15);
        options.Value.Expiry = timeSpan;
        var generator = new AccessCodeGenerator(options);
        string email = "test@test.com";
        var code = generator.GenerateCode(email);
        Assert.Equal(timeSpan, code.Expiry);
    }

    [Fact]
    public void generated_code_expiration_time_is_now_time_plus_timespan_from_options()
    {
        var timeSpan = TimeSpan.FromMinutes(15);
        IOptions<AccessCodeOptions> options = Options.Create<AccessCodeOptions>(new AccessCodeOptions());
        options.Value.Expiry = timeSpan;
        var generator = new AccessCodeGenerator(options);
        string email = "test@test.com";
        var code = generator.GenerateCode(email);
        Assert.InRange(code.ExpirationTime, DateTime.UtcNow + timeSpan - TimeSpan.FromSeconds(1), DateTime.UtcNow + timeSpan + TimeSpan.FromSeconds(1));
    }
}