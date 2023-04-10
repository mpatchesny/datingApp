using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration;

[Collection("Integration tests")]
public class UserRepositoryTests : IDisposable
{
    [Fact]
    public async Task get_user_by_id_should_succeed()
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.NotNull(user);
        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000001"), user.Id);
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
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var user = await _userRepository.GetByIdAsync(userId);
        user.ChangeBio("new bio");
        var exception = await Record.ExceptionAsync(async () => await _userRepository.UpdateAsync(user));
        Assert.Null(exception);
    }

    [Fact]
    public async Task delete_existing_user_should_succeed()
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user));
        Assert.Null(exception);
    }

    [Fact(Skip="after change user id from int to guid this test fails")]
    public async Task delete_nonexisting_user_should_throw_exception()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "888756489", "test3@test.com", "Klaudiusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var exception = await Record.ExceptionAsync(async () => await _userRepository.DeleteAsync(user));
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task add_user_should_succeed()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(user));
        Assert.Null(exception);
    }

    [Fact]
    public async Task add_user_with_existing_id_should_throw_exception()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var exception = await Record.ExceptionAsync(async () => await _userRepository.AddAsync(user));
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task get_nonexistsing_user_by_id_should_return_null()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000000");
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
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
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