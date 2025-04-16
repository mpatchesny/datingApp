using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Exceptions;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class SignUpHandlerTests
{
    [Fact]
    public async Task given_date_of_birth_is_not_ISO8601_format_SignUpHandler_throws_InvalidDateOfBirthFormatException()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(null));
        repository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));
        repository.Setup(x => x.GetByPhoneAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));

        var command = new SignUp(Guid.NewGuid(), "123456798", "test@test.com", "Janusz", "01-01-2000", 1, 2);
        var handler = new SignUpHandler(repository.Object);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));

        Assert.NotNull(exception);
        Assert.IsType<InvalidDateOfBirthFormatException>(exception);
    }

    [Fact]
    public async Task given_user_with_given_email_exists_SignUpHandler_throws_EmailAlreadyInUseException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(null));
        repository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(user));
        repository.Setup(x => x.GetByPhoneAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));

        var command = new SignUp(Guid.NewGuid(), "123456798", "test@test.com", "Janusz", "2000-01-01", 1, 2);
        var handler = new SignUpHandler(repository.Object);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));

        Assert.NotNull(exception);
        Assert.IsType<EmailAlreadyInUseException>(exception);
    }

    [Fact]
    public async Task given_user_with_given_phone_exists_SignUpHandler_throws_PhoneAlreadyInUseException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(null));
        repository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));
        repository.Setup(x => x.GetByPhoneAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(user));

        var command = new SignUp(Guid.NewGuid(), "123456798", "test@test.com", "Janusz", "2000-01-01", 1, 2);
        var handler = new SignUpHandler(repository.Object);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));

        Assert.NotNull(exception);
        Assert.IsType<PhoneAlreadyInUseException>(exception);
    }

    [Fact]
    public async Task given_user_with_given_email_or_phone_not_exists_SignUpHandler_should_succeed()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));
        repository.Setup(x => x.GetByPhoneAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));
        User addedUser = null;
        repository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => addedUser = u);

        var phone = "123456789";
        var email = "test@test.com";
        var name = "Janusz";
        var dateOfBirth = "2000-01-01";
        var sex = 1;
        var preferredSex = 2;
        var command = new SignUp(Guid.NewGuid(), phone, email, name, dateOfBirth, sex, preferredSex);
        var handler = new SignUpHandler(repository.Object);
     
        await handler.HandleAsync(command);
        repository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once());
        Assert.Equal(phone, addedUser.Phone);
        Assert.Equal(email, addedUser.Email);
        Assert.Equal(name, addedUser.Name);
        Assert.Equal(dateOfBirth, addedUser.DateOfBirth.Value.ToString("yyyy-MM-dd"));
        Assert.Equal(sex, (int) addedUser.Sex);
        Assert.Equal(preferredSex, (int) addedUser.Settings.PreferredSex);
        Assert.Equal(18, addedUser.Settings.PreferredAge.From);
        Assert.Equal(35, addedUser.Settings.PreferredAge.To);
        Assert.Equal(0.0, addedUser.Settings.Location.Lat);
        Assert.Equal(0.0, addedUser.Settings.Location.Lon);
        Assert.Equal(30, addedUser.Settings.PreferredMaxDistance.Value);
    }
}