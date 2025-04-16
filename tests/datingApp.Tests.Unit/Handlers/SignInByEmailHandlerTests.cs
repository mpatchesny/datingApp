using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.Security;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class SignUpByEmailHandlerTests
{
    [Fact]
    public async Task given_user_with_given_exmail_not_exists_SignInByEmailHandler_throws_InvalidCredentialsException()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);

        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new Mock<IAccessCodeVerificator>();
        verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        var handler = new SignInByEmailHandler(userRepository.Object, accessCodeStorage.Object, authenticator.Object, tokenStorage.Object, verificator.Object);
        var command = new SignInByEmail("test@test.com", "ABC");

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidCredentialsException>(exception);
    }

    [Fact]
    public async Task given_null_email_SignInByEmailHandler_throws_NoEmailProvidedException()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);

        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new Mock<IAccessCodeVerificator>();
        verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        var handler = new SignInByEmailHandler(userRepository.Object, accessCodeStorage.Object, authenticator.Object, tokenStorage.Object, verificator.Object);
        var command = new SignInByEmail(null, "ABC");

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<NoEmailProvidedException>(exception);
    }

    [Fact]
    public async Task given_null_access_code_provided_SignInByEmailHandler_throws_NoAccessCodeProvidedException()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);

        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new Mock<IAccessCodeVerificator>();
        verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        var handler = new SignInByEmailHandler(userRepository.Object, accessCodeStorage.Object, authenticator.Object, tokenStorage.Object, verificator.Object);
        var command = new SignInByEmail("test@test.com", null);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<NoAccessCodeProvidedException>(exception);
    }

    [Fact]
    public async Task given_access_code_not_in_storage_SignInByEmailHandlerl_throws_InvalidCredentialsException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user = new User(Guid.NewGuid(), "12345", "test@test.com", "Nazwa", new DateOnly(2000,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        
        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new Mock<IAccessCodeVerificator>();
        verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        var handler = new SignInByEmailHandler(userRepository.Object, accessCodeStorage.Object, authenticator.Object, tokenStorage.Object, verificator.Object);
        var command = new SignInByEmail("test@test.com", "ABC");

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidCredentialsException>(exception);
    }

    [Fact]
    public async Task given_negative_access_code_verification_SignInByEmailHandler_throws_InvalidCredentialsException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user = new User(settings.UserId, "12345", "test@test.com", "Nazwa", new DateOnly(2000,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        var code = CreateAccessCodeDto();
        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new Mock<IAccessCodeVerificator>();
        verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        var handler = new SignInByEmailHandler(userRepository.Object, accessCodeStorage.Object, authenticator.Object, tokenStorage.Object, verificator.Object);
        var command = new SignInByEmail("test@test.com", "ABC");

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidCredentialsException>(exception);
    }

    [Fact]
    public async Task given_valid_email_and_code_SignInByEmailHandler_should_succeed()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Male, new PreferredAge(18, 100), 100, new Location(0.0, 0.0));
        var user = new User(Guid.NewGuid(), "12345", "test@test.com", "Nazwa", new DateOnly(2000,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        var code = CreateAccessCodeDto();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        
        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);

        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);

        var verificator = new Mock<IAccessCodeVerificator>();
        verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) true);

        var handler = new SignInByEmailHandler(userRepository.Object, accessCodeStorage.Object, authenticator.Object, tokenStorage.Object, verificator.Object);
        var command = new SignInByEmail("test@test.com", "ABC");

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.Null(exception);
        authenticator.Verify(x => x.CreateToken(user.Id), Times.Once());
        tokenStorage.Verify(x => x.Set(It.IsAny<JwtDto>()), Times.Once());
    }

    private static AccessCodeDto CreateAccessCodeDto()
    {
        return new AccessCodeDto() {
            AccessCode ="ABC",
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromMinutes(15),
            Expiry = TimeSpan.FromMinutes(15)
        };
    }
}