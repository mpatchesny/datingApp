using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Services;
using Xunit;

namespace datingApp.Tests.Integration.Services;

public class DbPhotoDuplicateCheckerTests : IDisposable
{
    [Fact]
    public async void given_photo_with_given_checksum_exsits_for_given_user_IsDuplicate_returns_true()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var hashBytes = await _md5.ComputeHashAsync(stream);
        var checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto(checksum: checksum) };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var service = new DbPhotoDuplicateChecker(_dbContext);
        var isDuplicate = await service.IsDuplicate(user.Id, stream);

        Assert.True(isDuplicate);
    }

    [Fact]
    public async Task given_user_has_no_photos_IsDuplicate_returns_false()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var hashBytes = await _md5.ComputeHashAsync(stream);
        var checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var service = new DbPhotoDuplicateChecker(_dbContext);
        var isDuplicate = await service.IsDuplicate(user.Id, stream);

        Assert.False(isDuplicate);
    }

    [Fact]
    public async Task given_photo_with_given_checksum_notexsits_for_other_user_IsDuplicate_returns_false()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var hashBytes = await _md5.ComputeHashAsync(stream);
        var checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto(checksum: checksum) };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var stream2 = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var service = new DbPhotoDuplicateChecker(_dbContext);
        var isDuplicate = await service.IsDuplicate(user.Id, stream2);

        Assert.False(isDuplicate);
    }

    [Fact]
    public async Task given_photo_with_given_checksum_not_exsits_for_given_user_IsDuplicate_returns_false()
    {
        var stream1 = new MemoryStream(new byte[] { 1, 2, 3 });
        var hashBytes1 = await _md5.ComputeHashAsync(stream1);
        var checksum1 = BitConverter.ToString(hashBytes1).Replace("-", "").ToLowerInvariant();
        
        var stream2 = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var hashBytes2 = await _md5.ComputeHashAsync(stream2);
        var checksum2 = BitConverter.ToString(hashBytes2).Replace("-", "").ToLowerInvariant();

        var photos1 = new List<Photo>() { IntegrationTestHelper.CreatePhoto(checksum: checksum1) };
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos1);

        var photos2 = new List<Photo>() { IntegrationTestHelper.CreatePhoto(checksum: checksum2) };
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos2);

        _dbContext.ChangeTracker.Clear();

        var service = new DbPhotoDuplicateChecker(_dbContext);
        var isDuplicate = await service.IsDuplicate(user1.Id, stream2);

        Assert.False(isDuplicate);
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly System.Security.Cryptography.MD5 _md5;

    public DbPhotoDuplicateCheckerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _md5 = System.Security.Cryptography.MD5.Create();
    }

    public void Dispose()
    {
        _testDb.Dispose();
    }
}