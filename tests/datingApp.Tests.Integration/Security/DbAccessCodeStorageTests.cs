using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Infrastructure.Security;
using Xunit;

namespace datingApp.Tests.Integration.Security
{
    [Collection("Integration tests")]
    public class DbAccessCodeStorageTests : IDisposable
    {
        [Fact]
        public void if_access_code_with_given_email_exists_storage_should_return_code()
        {
            string email = "test@test.com";
            var code = new AccessCodeDto()
            {
                AccessCode = "12345",
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow,
                Expiry = TimeSpan.FromMinutes(15)
            };

            var storage = new DbAccessCodeStorage(_testDb.DbContext);
            storage.Set(code);
            Assert.Equal(code, storage.Get(email));
        }

        [Fact]
        public void set_code_for_same_email_twice_or_more_times_should_succeed_and_last_setted_code_should_be_fetched()
        {
            string email = "test@test.com";
            var code1 = new AccessCodeDto()
            {
                AccessCode = "12345",
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow,
                Expiry = TimeSpan.FromMinutes(15)
            };

            var code2 = new AccessCodeDto()
            {
                AccessCode = "56789",
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow,
                Expiry = TimeSpan.FromMinutes(15)
            };

            var storage = new DbAccessCodeStorage(_testDb.DbContext);
            storage.Set(code1);
            storage.Set(code2);
            Assert.Equal(code2, storage.Get(email));
        }

        [Fact]
        public void given_access_code_with_given_email_exists_storage_should_return_code_even_after_expiration_time()
        {
            string email = "test@test.com";
            var code = new AccessCodeDto()
            {
                AccessCode = "12345",
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow,
                Expiry = TimeSpan.FromSeconds(1)
            };

            var storage = new DbAccessCodeStorage(_testDb.DbContext);
            storage.Set(code);

            System.Threading.Thread.Sleep(1000);

            Assert.Equal(code, storage.Get(email));
        }

        [Fact]
        public void if_access_code_with_given_email_not_exists_storage_should_return_null()
        {
            string email = "test@test.com";
            var code = new AccessCodeDto()
            {
                AccessCode = "12345",
                EmailOrPhone = email,
                ExpirationTime = DateTime.UtcNow,
                Expiry = TimeSpan.FromMinutes(15)
            };

            string badEmail = "test1@test.com";
            var storage = new DbAccessCodeStorage(_testDb.DbContext);
            Assert.Null(storage.Get(badEmail));
        }

        // Arrange
        private readonly TestDatabase _testDb;

        public DbAccessCodeStorageTests()
        {
            _testDb = new TestDatabase();
        }

        // Teardown
        public void Dispose()
        {
            _testDb.Dispose();
        }
    }
}
