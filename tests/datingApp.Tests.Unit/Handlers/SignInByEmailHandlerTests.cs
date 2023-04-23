using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class SignUpByEmailHandlerTests
{
    [Fact]
    public async Task given_nonexisting_email_sign_in_by_email_should_throw_exception()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);

        var codeStorage = new Mock<IAccessCodeStorage>();
        codeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new Mock<AccessCodeVerificator>();

        SignInByEmailHandler handler = new SignInByEmailHandler(userRepository.Object, codeStorage.Object, authenticator.Object, tokenStorage.Object, verificator.Object);
        SignInByEmail command = new SignInByEmail("test@test.com", "ABC");
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidCredentialsException>(exception);
    }

    [Fact]
    public async Task given_no_access_code_in_storage_sign_in_by_email_should_throw_exception()
    {
        var settings = new UserSettings(Guid.NewGuid(), Sex.Male, 18, 100, 100, 0.0, 0.0);
        var user = new User(Guid.NewGuid(), "12345", "test@test.com", "Nazwa", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        var codeStorage = new Mock<IAccessCodeStorage>();
        codeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new Mock<AccessCodeVerificator>();

        SignInByEmailHandler handler = new SignInByEmailHandler(userRepository.Object, codeStorage.Object, authenticator.Object, tokenStorage.Object, verificator.Object);
        SignInByEmail command = new SignInByEmail("test@test.com", "ABC");
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidCredentialsException>(exception);
    }

    [Fact]
    public async Task given_negative_access_code_verification_sign_in_by_email_should_throw_exception()
    {
        var settings = new UserSettings(Guid.NewGuid(), Sex.Male, 18, 100, 100, 0.0, 0.0);
        var user = new User(Guid.NewGuid(), "12345", "test@test.com", "Nazwa", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        var code = new AccessCodeDto() {
            AccessCode ="12345",
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow,
            Expiry = TimeSpan.FromMinutes(15)
        };
        var codeStorage = new Mock<IAccessCodeStorage>();
        codeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new AccessCodeVerificator();

        SignInByEmailHandler handler = new SignInByEmailHandler(userRepository.Object, codeStorage.Object, authenticator.Object, tokenStorage.Object, verificator);
        SignInByEmail command = new SignInByEmail("test@test.com", "ABC");
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidCredentialsException>(exception);
    }

    [Fact]
    public async Task given_valid_email_and_code_sign_in_by_email_should_succeed()
    {
        var settings = new UserSettings(Guid.NewGuid(), Sex.Male, 18, 100, 100, 0.0, 0.0);
        var user = new User(Guid.NewGuid(), "12345", "test@test.com", "Nazwa", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        var code = new AccessCodeDto() {
            AccessCode ="ABC",
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromMinutes(15),
            Expiry = TimeSpan.FromMinutes(15)
        };
        var codeStorage = new Mock<IAccessCodeStorage>();
        codeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new AccessCodeVerificator();

        SignInByEmailHandler handler = new SignInByEmailHandler(userRepository.Object, codeStorage.Object, authenticator.Object, tokenStorage.Object, verificator);
        SignInByEmail command = new SignInByEmail("test@test.com", "ABC");
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.Null(exception);
    }
}