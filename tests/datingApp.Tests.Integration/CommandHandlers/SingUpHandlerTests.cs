using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

[Collection("Integration tests")]
public class SingUpHandlerTests : IDisposable
{
    [Fact]
    public async Task signup_user_with_existing_email_should_throw_exception()
    {
        var command = new SignUp("111111111", "test@test.com", "Janusz", "2000-01-01", 1, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<EmailAlreadyInUseException>(exception);
    }
    
    [Fact]
    public async Task signup_user_with_existing_phone_should_throw_exception()
    {
        var command = new SignUp("123456789", "freeemail@test.com", "Janusz", "2000-01-01", 1, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhoneAlreadyInUseException>(exception);
    }

    [Fact]
    public async Task signup_user_with_free_phone_and_free_email_should_succeed()
    {
        var command = new SignUp("111111111", "freeemail@test.com", "Januesz", "2000-01-01", 1, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_invalid_date_of_birth_singup_should_throw_exception()
    {
        var command = new SignUp("111111111", "freeemail@test.com", "Januesz", "01.01.2000", 1, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDateOfBirthFormatException>(exception);
    }

    // Arrange
    private readonly SignUpHandler _handler;
    private readonly TestDatabase _testDb;
    public SingUpHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        var userRepository = new PostgresUserRepository(_testDb.DbContext);
        _handler = new SignUpHandler(userRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}