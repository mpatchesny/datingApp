using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class SingUpHandlerTests : IDisposable
{
    [Fact]
    public async Task given_email_already_exists_signup_user_throws_EmailAlreadyInUseException()
    {
        var email = "test@test.com";
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb, email);

        var command = new SignUp(Guid.NewGuid(), "111111111", email, "Janusz", "2000-01-01", 1, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<EmailAlreadyInUseException>(exception);
    }
    
    [Fact]
    public async Task given_phone_already_exists_signup_user_throws_PhoneAlreadyInUseException()
    {
        var phone = "123456789";
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb, phone: phone);

        var command = new SignUp(Guid.NewGuid(), phone, "test@test.com", "Janusz", "2000-01-01", 1, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhoneAlreadyInUseException>(exception);
    }

    [Fact]
    public async Task signup_user_with_free_phone_and_free_email_should_succeed()
    {
        var command = new SignUp(Guid.NewGuid(), "123456789", "test@test.com", "Januesz", "2000-01-01", 1, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_invalid_date_of_birth_singup_throws_InvalidDateOfBirthFormatException()
    {
        var command = new SignUp(Guid.NewGuid(), "123456789", "test@test.com", "Januesz", "01.01.2000", 1, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDateOfBirthFormatException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly SignUpHandler _handler;
    public SingUpHandlerTests()
    {
        _testDb = new TestDatabase();
        var userRepository = new DbUserRepository(_testDb.DbContext);
        _handler = new SignUpHandler(userRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}