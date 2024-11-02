using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class UserRepositoryTests : IDisposable
{
    [Fact]
    public async Task given_user_exists_and_has_photos_get_user_by_id_should_return_proper_user_with_photos()
    {
        var photos = new List<Photo>() {
            IntegrationTestHelper.CreatePhoto(),
            IntegrationTestHelper.CreatePhoto(),
            IntegrationTestHelper.CreatePhoto()
        };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();
        
        var retrievedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.True(user.IsEqualTo(retrievedUser));
    }

    [Fact]
    public async Task given_user_exists_and_has_no_photos_get_user_by_id_should_return_proper_user_without_photos()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var retrievedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.True(user.IsEqualTo(retrievedUser));
    }

    [Fact]
    public async Task given_user_exists_and_has_photos_get_user_by_photo_id_should_succeed()
    {
        var photos = new List<Photo>() {
            IntegrationTestHelper.CreatePhoto(),
            IntegrationTestHelper.CreatePhoto(),
            IntegrationTestHelper.CreatePhoto()
        };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        foreach (var photo in photos)
        {
            var retrievedUser = await _userRepository.GetByPhotoIdAsync(photo.Id);
            Assert.True(user.IsEqualTo(retrievedUser));
        }
    }

    [Fact]
    public async Task given_user_exists_get_user_by_nonexisting_photo_id_should_return_null()
    {
        var photos = new List<Photo>() {
            IntegrationTestHelper.CreatePhoto(),
            IntegrationTestHelper.CreatePhoto(),
            IntegrationTestHelper.CreatePhoto()
        };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var retrievedUser = await _userRepository.GetByPhotoIdAsync(Guid.NewGuid());
        Assert.Null(retrievedUser);
    }

    [Fact]
    public async Task given_user_exists_get_user_by_phone_should_succeed()
    {
        var phone = "123456789";
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, phone : phone);
        _dbContext.ChangeTracker.Clear();

        var retrievedUser = await _userRepository.GetByPhoneAsync(phone);
        Assert.True(user.IsEqualTo(retrievedUser));
    }

    [Fact]
    public async Task given_user_exists_get_user_by_email_should_succeed()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, email : email);
        _dbContext.ChangeTracker.Clear();

        var retrievedUser = await _userRepository.GetByEmailAsync(email);
        Assert.True(user.IsEqualTo(retrievedUser));
    }

    [Fact]
    public async Task update_existing_user_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        user.ChangeBio("new bio");

        await _userRepository.UpdateAsync(user);
        _dbContext.ChangeTracker.Clear();

        var updatedUser = await _dbContext.Users.Include(u => u.Photos).Include(u => u.Settings).FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.True(user.IsEqualTo(updatedUser));
    }

    [Fact]
    public async Task given_user_photo_is_added_update_user_should_update_photos()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var photo = IntegrationTestHelper.CreatePhoto();
        user.AddPhoto(photo);

        await _userRepository.UpdateAsync(user);
        _dbContext.ChangeTracker.Clear();

        var updatedUser = await _dbContext.Users.Include(u => u.Photos).Include(u => u.Settings).FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.True(user.IsEqualTo(updatedUser));
    }

    [Fact]
    public async Task given_user_photo_is_removed_update_user_should_update_photos()
    {
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto() };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos);
        user.RemovePhoto(photos[0].Id);

        await _userRepository.UpdateAsync(user);
        _dbContext.ChangeTracker.Clear();

        var updatedUser = await _dbContext.Users.Include(u => u.Photos).Include(u => u.Settings).FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.True(user.IsEqualTo(updatedUser));
    }

    [Fact]
    public async Task given_user_photo_oridinal_change_update_user_should_update_photos()
    {
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto(0), IntegrationTestHelper.CreatePhoto(1) };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos);
        user.ChangeOridinal(photos[1].Id, 0);

        await _userRepository.UpdateAsync(user);
        _dbContext.ChangeTracker.Clear();

        var updatedUser = await _dbContext.Users.Include(u => u.Photos).Include(u => u.Settings).FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.Collection(updatedUser.Photos.OrderBy(p => p.Oridinal.Value), 
            p => Assert.Equal(p.Id, photos[1].Id),
            p => Assert.Equal(p.Id, photos[0].Id)
        );
    }

    [Fact]
    public async Task delete_existing_user_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        await _userRepository.DeleteAsync(user);
        _dbContext.ChangeTracker.Clear();

        var deletedUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task delete_nonexisting_user_throws_exception()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 50, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task add_user_should_succeed()
    {
        var user = IntegrationTestHelper.CreateUser();
        await _userRepository.AddAsync(user);

        _dbContext.ChangeTracker.Clear();
        var addedUser = await _dbContext.Users
                            .Include(u => u.Settings)
                            .Include(u => u.Photos)
                            .FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.True(user.IsEqualTo(addedUser));
    }

    [Fact]
    public async Task add_user_with_existing_id_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var settings = new UserSettings(user.Id, PreferredSex.Female, new PreferredAge(18, 20), 50, new Location(45.5, 45.5));
        var badUser = new User(user.Id, "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(badUser));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task add_user_with_existing_email_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var badUser = IntegrationTestHelper.CreateUser(email: user.Email);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(badUser));
        Assert.NotNull(exception);
    }

    [Fact (Skip = "phone is not unique on db level")]
    public async Task add_user_with_existing_phone_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var badUser = IntegrationTestHelper.CreateUser(phone: user.Phone);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(badUser));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task given_user_not_exists_get_user_by_id_should_return_null()
    {
        var userId = Guid.NewGuid();
        var user = await _userRepository.GetByIdAsync(userId);
        Assert.Null(user);
    }

    [Fact]
    public async Task given_user_not_exists_get_user_by_phone_should_return_null()
    {
        var phone = "000555000";
        var user = await _userRepository.GetByPhoneAsync(phone);
        Assert.Null(user);
    }

    [Fact]
    public async Task given_user_not_exists_get_user_by_email_should_return_null()
    {
        var email = "bademail@test.com";
        var user = await _userRepository.GetByEmailAsync(email);
        Assert.Null(user);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _userRepository = new DbUserRepository(_dbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}