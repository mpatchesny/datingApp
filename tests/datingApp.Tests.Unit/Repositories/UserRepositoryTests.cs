using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Unit.Repositories;

public class RepositoriesTest
{
    [Fact]
    public void get_user_by_id_should_succeed()
    {
        var task =  _userRepository.GetByIdAsync(1);
        task.Wait();
        var user = task.Result;
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
    }

    [Fact]
    public void get_user_by_phone_should_succeed()
    {
        var phone = "123456789";
        var task =  _userRepository.GetByPhoneAsync(phone);
        task.Wait();
        var user = task.Result;
        Assert.NotNull(user);
        Assert.Equal(phone, user.Phone);
    }

    [Fact]
    public void get_user_by_email_should_succeed()
    {
        var email = "test@test.com";
        var task =  _userRepository.GetByEmailAsync(email);
        task.Wait();
        var user = task.Result;
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void update_user_should_succeed()
    {
        var userId = 1;
        var task = _userRepository.GetByIdAsync(userId);
        task.Wait();
        var user = task.Result;
        user.ChangeBio("new bio");

        var updateTask = _userRepository.UpdateAsync(user);
        updateTask.Wait();

        task = _userRepository.GetByIdAsync(userId);
        task.Wait();
        user = task.Result;

        Assert.Equal("new bio", user.Bio);
    }

    [Fact]
    public void delete_user_should_succeed()
    {
        var userId = 1;
        var task = _userRepository.GetByIdAsync(userId);
        task.Wait();
        var user = task.Result;
        var deleteTask =  _userRepository.DeleteAsync(user);
        deleteTask.Wait();

        task = _userRepository.GetByIdAsync(userId);
        task.Wait();
        user = task.Result;
        Assert.Null(user);
    }

    [Fact]
    public void add_user_should_succeed()
    {
        var settings = new UserSettings(1, Sex.Female, new AgeRange(18, 21), 20);
        var newId = 5;
        var user = new User(newId, "000000000", "test2@test.com", "Klaudiusz", new DateOnly(2000,1,1), Sex.Male, null, null, settings);
        var addTask =  _userRepository.AddAsync(user);
        addTask.Wait();

        var task = _userRepository.GetByIdAsync(newId);
        task.Wait();
        user = task.Result;
        Assert.NotNull(user);
        Assert.Equal(newId, user.Id);
    }

    [Fact]
    public void get_nonexistsing_user_by_id_should_return_null()
    {
        var userId = 0;
        var task = _userRepository.GetByIdAsync(userId);
        task.Wait();
        var user = task.Result;
        Assert.Null(user);
    }

    [Fact]
    public void get_nonexistsing_user_by_phone_should_return_null()
    {
        var phone = "000555000";
        var task = _userRepository.GetByPhoneAsync(phone);
        task.Wait();
        var user = task.Result;
        Assert.Null(user);
    }

    [Fact]
    public void get_nonexistsing_user_by_email_should_return_null()
    {
        var email = "nonexistingemail@test.com";
        var task = _userRepository.GetByEmailAsync(email);
        task.Wait();
        var user = task.Result;
        Assert.Null(user);
    }

    // Arrange
    private readonly IUserRepository _userRepository;
    public RepositoriesTest()
    {
        var users = new List<User>();
        var settings = new UserSettings(1, Sex.Female, new AgeRange(18, 21), 20);
        users.Add(new User(1, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, null, settings));
        settings = new UserSettings(2, Sex.Female, new AgeRange(30, 35), 20);
        users.Add(new User(2, "000111222", "test2@test.com", "Mariusz", new DateOnly(2000,1,1), Sex.Male, null, null, settings));
        _userRepository = new InMemoryUserRepository(users);
    }
}