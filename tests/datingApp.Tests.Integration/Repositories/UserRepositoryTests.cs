using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration;

public class UserRepositoryTests : IDisposable
{
    [Fact]
    public async Task get_user_by_id_should_succeed()
    {
        var user = await _userRepository.GetByIdAsync(1);
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
    }

    [Fact]
    public async Task get_user_by_existing_phone_should_succeed()
    {
        var phone = "123456789";
        var user = await _userRepository.GetByPhoneAsync(phone);
        Assert.NotNull(user);
        Assert.Equal(phone, user.Phone);
    }

    [Fact]
    public async Task get_user_by_existing_email_should_succeed()
    {
        var email = "test@test.com";
        var user = await _userRepository.GetByEmailAsync(email);
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public async Task update_existing_user_should_succeed()
    {
        var userId = 1;
        var user = await _userRepository.GetByIdAsync(userId);
        user.ChangeBio("new bio");
        await _userRepository.UpdateAsync(user);
        _testDb.DbContext.SaveChanges();

        var user2 = await _userRepository.GetByIdAsync(userId);
        Assert.Equal("new bio", user2.Bio);
    }

    [Fact]
    public async Task update_nonexisting_user_should_fail()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(0, "111111111", "bademail@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        await _userRepository.UpdateAsync(user);
        _testDb.DbContext.SaveChanges();
        // FIXME
        // Assert.Equal(true, updateTask.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task delete_existing_user_should_succeed()
    {
        var userId = 1;
        await _userRepository.DeleteAsync(userId);
        _testDb.DbContext.SaveChanges();
        var user2 = await _userRepository.GetByIdAsync(userId);
        Assert.Null(user2);
    }

    [Fact]
    public async Task delete_nonexisting_user_should_fail()
    {
        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(999));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task add_user_should_succeed()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 45.5, 45.5);
        var user = new User(0, "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        await  _userRepository.AddAsync(user);
        _testDb.DbContext.SaveChanges();

        var user2 = await _userRepository.GetByIdAsync(user.Id);
        Assert.NotNull(user2);
    }

    [Fact]
    public async Task get_nonexistsing_user_by_id_should_return_null()
    {
        var userId = 0;
        var user = await _userRepository.GetByIdAsync(userId);
        Assert.Null(user);
    }

    [Fact]
    public async Task get_nonexistsing_user_by_phone_should_return_null()
    {
        var phone = "000555000";
        var user = await _userRepository.GetByPhoneAsync(phone);
        Assert.Null(user);
    }

    [Fact]
    public async Task get_nonexistsing_user_by_email_should_return_null()
    {
        var email = "nonexistingemail@test.com";
        var user = await _userRepository.GetByEmailAsync(email);
        Assert.Null(user);
    }

    // Arrange
    private readonly IUserRepository _userRepository;
    private readonly TestDatabase _testDb;

    public UserRepositoryTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _userRepository = new PostgresUserRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}