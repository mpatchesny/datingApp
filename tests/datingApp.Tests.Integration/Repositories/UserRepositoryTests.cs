using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class UserRepositoryTests : IDisposable
{
    [Fact]
    public async Task given_user_exists_get_user_by_id_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        
        var retrievedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.Same(user, retrievedUser);
    }

    [Fact]
    public async Task given_user_exists_get_user_by_phone_should_succeed()
    {
        var phone = "123456789";
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb, phone : phone);

        var retrievedUser = await _userRepository.GetByPhoneAsync(phone);
        Assert.Same(user, retrievedUser);
    }

    [Fact]
    public async Task given_user_exists_get_user_by_email_should_succeed()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb, email : email);

        var retrievedUser = await _userRepository.GetByEmailAsync(email);
        Assert.Same(user, retrievedUser);
    }

    [Fact]
    public async Task update_existing_user_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        user.ChangeBio("new bio");

        var exception = await Record.ExceptionAsync(async () => await _userRepository.UpdateAsync(user));
        Assert.Null(exception);
        var updatedUser = await _testDb.DbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.Same(user, updatedUser);
    }

    [Fact]
    public async Task given_user_exists_delete_user_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user));
        Assert.Null(exception);
        var deletedUser = await _testDb.DbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task given_user_not_exists_delete_user_throws_exception()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, 18, 20, 50, 45.5, 45.5);
        var user = new User(settings.UserId, "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task add_user_should_succeed()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, 18, 20, 50, 45.5, 45.5);
        var user = new User(settings.UserId, "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(user));
        Assert.Null(exception);
        var addedUser = await _testDb.DbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.Same(addedUser, user);
    }

    [Fact]
    public async Task add_user_with_existing_id_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var settings = new UserSettings(user.Id, PreferredSex.Female, 18, 20, 50, 45.5, 45.5);
        var badUser = new User(user.Id, "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(badUser));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task add_user_with_existing_email_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, 18, 20, 50, 45.5, 45.5);
        var badUser = new User(settings.UserId, "000000000", user.Email, "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(badUser));
        Assert.NotNull(exception);
    }

    [Fact (Skip = "email is not unique apparently")]
    public async Task add_user_with_existing_phone_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, 18, 20, 50, 45.5, 45.5);
        var badUser = new User(settings.UserId, user.Phone, "test@test.com", "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

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

    [Fact]
    public async Task given_user_exists_and_have_match_delete_user_deletes_match()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user1));
        Assert.Null(exception);

        var matchExists = await _testDb.DbContext.Matches.AnyAsync(m => m.Id == match.Id);
        Assert.False(matchExists);
    }

    [Fact]
    public async Task given_user_exists_and_have_match_with_message_delete_user_deletes_match_with_message()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message  = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user1));
        Assert.Null(exception);

        var matchExists = await _testDb.DbContext.Matches.AnyAsync(m => m.Id == match.Id);
        Assert.False(matchExists);

        var messageExists = await _testDb.DbContext.Messages.AnyAsync(m => m.Id == message.Id);
        Assert.False(messageExists);
    }

    [Fact]
    public async Task given_user_exists_and_have_swipe_delete_user_not_deletes_swipe()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var swipe = await IntegrationTestHelper.CreateSwipeAsync(_testDb, user2.Id, user1.Id, Like.Like);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user1));
        Assert.Null(exception);

        var swipeExists = await _testDb.DbContext.Swipes.AnyAsync(s => s.SwipedWhoId == user1.Id);
        Assert.True(swipeExists);
    }

    [Fact]
    public async Task given_user_exists_and_have_photos_delete_user_deletes_photos()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo1 = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, 1);
        var photo2 = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, 2);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user));
        Assert.Null(exception);

        var photoExists = await _testDb.DbContext.Photos.AnyAsync(p => p.Id == photo1.Id);
        Assert.False(photoExists);

        photoExists = await _testDb.DbContext.Photos.AnyAsync(p => p.Id == photo2.Id);
        Assert.False(photoExists);
    }

    [Fact]
    public async Task given_user_exists_and_have_photos_and_photo_files_delete_user_deletes_photos_and_photo_files()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo1 = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, 1);
        var photo2 = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, 2);
        var file1 = await IntegrationTestHelper.CreateFileAsync(_testDb, photo1.Id);
        var file2 = await IntegrationTestHelper.CreateFileAsync(_testDb, photo2.Id);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user));
        Assert.Null(exception);

        var photoExists = await _testDb.DbContext.Photos.AnyAsync(p => p.Id == photo1.Id);
        Assert.False(photoExists);

        photoExists = await _testDb.DbContext.Photos.AnyAsync(p => p.Id == photo2.Id);
        Assert.False(photoExists);

        var fileExists = await _testDb.DbContext.Files.AnyAsync(f => f.Id == photo1.Id);
        Assert.False(fileExists);

        fileExists = await _testDb.DbContext.Files.AnyAsync(f => f.Id == photo2.Id);
        Assert.False(photoExists);
    }

    // Arrange
    private readonly IUserRepository _userRepository;
    private readonly TestDatabase _testDb;

    public UserRepositoryTests()
    {
        _testDb = new TestDatabase();
        _userRepository = new DbUserRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}