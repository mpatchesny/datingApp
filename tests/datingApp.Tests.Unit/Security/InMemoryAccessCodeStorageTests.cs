using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Infrastructure.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Security;

public class InMemoryAccessCodeStorageTests
{
    [Fact]
    public void if_access_code_with_given_email_exists_storage_should_return_code()
    {
        string email = "test@test.com";
        var code = new AccessCodeDto() {
            AccessCode ="12345",
            EmailOrPhone = email,
            ExpirationTime = DateTime.UtcNow,
            Expiry = TimeSpan.FromMinutes(15)
        };

        var storage = new InMemoryAccessCodeStorage(_memoryCache);
        storage.Set(code);
        Assert.Equal(code, storage.Get(email));
    }

    [Fact]
    public void if_access_code_with_given_email_exists_storage_should_return_null_after_expiration_time()
    {
        string email = "test@test.com";
        var code = new AccessCodeDto() {
            AccessCode ="12345",
            EmailOrPhone = email,
            ExpirationTime = DateTime.UtcNow,
            Expiry = TimeSpan.FromSeconds(1)
        };

        var storage = new InMemoryAccessCodeStorage(_memoryCache);
        storage.Set(code);

        System.Threading.Thread.Sleep(1000);

        Assert.Null(storage.Get(email));
    }

    [Fact]
    public void if_access_code_with_given_email_not_exists_storage_should_return_null()
    {
        string email = "test@test.com";
        var code = new AccessCodeDto() {
            AccessCode ="12345",
            EmailOrPhone = email,
            ExpirationTime = DateTime.UtcNow,
            Expiry = TimeSpan.FromMinutes(15)
        };

        string badEmail = "test1@test.com";
        var storage = new InMemoryAccessCodeStorage(_memoryCache);
        Assert.Null(storage.Get(badEmail));
    }

    private readonly IMemoryCache _memoryCache;
    public InMemoryAccessCodeStorageTests()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        _memoryCache = serviceProvider.GetService<IMemoryCache>();
    }
}