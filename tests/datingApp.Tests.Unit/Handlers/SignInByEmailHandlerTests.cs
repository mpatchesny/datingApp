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
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.Security;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class SignUpByEmailHandlerTests
{
    [Fact]
    public async Task given_nonexisting_email_sign_in_by_email_should_throw_exception()
    {
        _userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);
        _tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);
        _verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        SignInByEmailHandler handler = new SignInByEmailHandler(_userRepository.Object, _accessCodeStorage.Object, _authenticator.Object, _tokenStorage.Object, _verificator.Object);
        SignInByEmail command = new SignInByEmail("test@test.com", "ABC");
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidCredentialsException>(exception);
    }

    [Fact]
    public async Task given_null_email_provided_sign_in_by_email_should_throw_NoEmailProvidedException()
    {
        _userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);
        _tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);
        _verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        SignInByEmailHandler handler = new SignInByEmailHandler(_userRepository.Object, _accessCodeStorage.Object, _authenticator.Object, _tokenStorage.Object, _verificator.Object);
        SignInByEmail command = new SignInByEmail(null, "ABC");
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<NoEmailProvidedException>(exception);
    }

    [Fact]
    public async Task given_null_access_code_provided_sign_in_by_email_should_throw_NoAccessCodeProvidedException()
    {
        _userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);
        _tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);
        _verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        SignInByEmailHandler handler = new SignInByEmailHandler(_userRepository.Object, _accessCodeStorage.Object, _authenticator.Object, _tokenStorage.Object, _verificator.Object);
        SignInByEmail command = new SignInByEmail("test@test.com", null);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<NoAccessCodeProvidedException>(exception);
    }

    [Fact]
    public async Task given_no_access_code_in_storage_sign_in_by_email_should_throw_exception()
    {
        var settings = new UserSettings(Guid.NewGuid(), Sex.Male, 18, 100, 100, 0.0, 0.0);
        var user = new User(Guid.NewGuid(), "12345", "test@test.com", "Nazwa", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns((AccessCodeDto) null);
        _tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);
        _verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        SignInByEmailHandler handler = new SignInByEmailHandler(_userRepository.Object, _accessCodeStorage.Object, _authenticator.Object, _tokenStorage.Object, _verificator.Object);
        SignInByEmail command = new SignInByEmail("test@test.com", "ABC");
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidCredentialsException>(exception);
    }

    [Fact]
    public async Task given_negative_access_code_verification_sign_in_by_email_should_throw_exception()
    {
        var settings = new UserSettings(Guid.NewGuid(), Sex.Male, 18, 100, 100, 0.0, 0.0);
        var user = new User(settings.UserId, "12345", "test@test.com", "Nazwa", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        var code = CreateAccessCodeDto();
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        _tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);
        _verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) false);

        SignInByEmailHandler handler = new SignInByEmailHandler(_userRepository.Object, _accessCodeStorage.Object, _authenticator.Object, _tokenStorage.Object, _verificator.Object);
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
        _userRepository.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        var code = CreateAccessCodeDto();
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        _tokenStorage.Setup(m => m.Get()).Returns((JwtDto) null);
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns((JwtDto) null);
        _verificator.Setup(m => m.Verify(It.IsAny<AccessCodeDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns((bool) true);

        SignInByEmailHandler handler = new SignInByEmailHandler(_userRepository.Object, _accessCodeStorage.Object, _authenticator.Object, _tokenStorage.Object, _verificator.Object);
        SignInByEmail command = new SignInByEmail("test@test.com", "ABC");
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.Null(exception);
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

    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IAccessCodeStorage> _accessCodeStorage;
    private readonly Mock<ITokenStorage> _tokenStorage;
    private readonly Mock<IAuthenticator> _authenticator;
    private readonly Mock<IAccessCodeVerificator> _verificator;
    public SignUpByEmailHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _accessCodeStorage = new Mock<IAccessCodeStorage>();
        _tokenStorage = new Mock<ITokenStorage>();
        _authenticator = new Mock<IAuthenticator>();
        _verificator = new Mock<IAccessCodeVerificator>();
    }
}