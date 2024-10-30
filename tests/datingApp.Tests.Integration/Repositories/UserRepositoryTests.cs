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
    public async Task given_user_photos_change_update_user_should_update_photos_1()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        user.AddPhoto(new Photo(Guid.NewGuid(), user.Id, "abcdef", 0));

        await _userRepository.UpdateAsync(user);
        var updatedUser = await _testDb.DbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.Same(user, updatedUser);
    }

    [Fact]
    public async Task given_user_photos_change_update_user_should_update_photos_2()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, 0);
        user.RemovePhoto(photo.Id);

        await _userRepository.UpdateAsync(user);
        var updatedUser = await _testDb.DbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.Same(user, updatedUser);
    }

    [Fact]
    public async Task given_user_photos_change_update_user_should_update_photos_3()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo1 = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, 0);
        var photo2 = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, 1);
        user.ChangeOridinal(photo2.Id, 0);

        await _userRepository.UpdateAsync(user);
        var updatedUser = await _testDb.DbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        var photos = updatedUser.Photos.OrderBy(p => p.Oridinal.Value).ToList();
        Assert.Collection(photos, 
            p => Assert.Equal(p.Id, photo2.Id),
            p => Assert.Equal(p.Id, photo1.Id)
        );
    }

    [Fact]
    public async Task delete_existing_user_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user));
        Assert.Null(exception);
        var deletedUser = await _testDb.DbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
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
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 50, new Location(45.5, 45.5));
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
        var settings = new UserSettings(user.Id, PreferredSex.Female, new PreferredAge(18, 20), 50, new Location(45.5, 45.5));
        var badUser = new User(user.Id, "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(badUser));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task add_user_with_existing_email_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 50, new Location(45.5, 45.5));
        var badUser = new User(settings.UserId, "000000000", user.Email, "Klaudiusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(badUser));
        Assert.NotNull(exception);
    }

    [Fact (Skip = "email is not unique apparently")]
    public async Task add_user_with_existing_phone_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 50, new Location(45.5, 45.5));
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