using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Notifications;
using datingApp.Application.Security;
using datingApp.Application.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class RequestEmailAccessCodeHandlerTests
{
    [Fact]
    public async Task given_email_address_valid_RequestEmailAccessCodeHandler_should_succeed()
    {
        var code = CreateAccessCodeDto();
        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        accessCodeStorage.Setup(m => m.Set(code));

        var accessCodeGenerator = new Mock<IAccessCodeGenerator>();
        accessCodeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);

        var emailNotificationSender =  new Mock<INotificationSender<Email>>();
        emailNotificationSender.Setup(m => m.SendAsync(It.IsAny<Email>()));

        var emailMsg = new Email("sender", "receiver", "subject", "text body", "html body");
        var emailMessageGenerator = new Mock<INotificationMessageGenerator<Email>>();
        emailMessageGenerator.Setup(m => m.Generate()).Returns(emailMsg);

        var emailGeneratorFactory = new Mock<IEmailGeneratorFactory>();
        emailGeneratorFactory.Setup(m => m.CreateAccessCodeEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(emailMessageGenerator.Object);

        string email = "test@test.com";
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(accessCodeGenerator.Object, accessCodeStorage.Object, emailNotificationSender.Object, emailGeneratorFactory.Object);
        await handler.HandleAsync(command);

        accessCodeStorage.Verify(mock => mock.Set(code), Times.Once());
        accessCodeGenerator.Verify(mock => mock.GenerateCode(email), Times.Once());
        emailGeneratorFactory.Verify(mock => mock.CreateAccessCodeEmail(email, code.AccessCode, code.Expiry), Times.Once());
        emailNotificationSender.Verify(mock => mock.SendAsync(emailMsg), Times.Once());
    }

    [Fact]
    public async Task given_email_address_is_null_RequestEmailAccessCodeHandler_throws_NoEmailProvidedException()
    {
        var code = CreateAccessCodeDto();
        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        accessCodeStorage.Setup(m => m.Set(code));

        var accessCodeGenerator = new Mock<IAccessCodeGenerator>();
        accessCodeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);

        var emailNotificationSender =  new Mock<INotificationSender<Email>>();
        emailNotificationSender.Setup(m => m.SendAsync(It.IsAny<Email>()));

        var emailMsg = new Email("sender", "receiver", "subject", "text body", "html body");
        var emailMessageGenerator = new Mock<INotificationMessageGenerator<Email>>();
        emailMessageGenerator.Setup(m => m.Generate()).Returns(emailMsg);

        var emailGeneratorFactory = new Mock<IEmailGeneratorFactory>();
        emailGeneratorFactory.Setup(m => m.CreateAccessCodeEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(emailMessageGenerator.Object);

        string email = null;
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(accessCodeGenerator.Object, accessCodeStorage.Object, emailNotificationSender.Object, emailGeneratorFactory.Object);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));

        Assert.NotNull(exception);
        Assert.IsType<NoEmailProvidedException>(exception);
    }

    [Fact]
    public async Task given_invalid_email_RequestEmailAccessCodeHandler_throws_InvalidEmailException()
    {
        var code = CreateAccessCodeDto();
        var accessCodeStorage = new Mock<IAccessCodeStorage>();
        accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        accessCodeStorage.Setup(m => m.Set(code));

        var accessCodeGenerator = new Mock<IAccessCodeGenerator>();
        accessCodeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);

        var emailNotificationSender =  new Mock<INotificationSender<Email>>();
        emailNotificationSender.Setup(m => m.SendAsync(It.IsAny<Email>()));

        var emailMsg = new Email("sender", "receiver", "subject", "text body", "html body");
        var emailMessageGenerator = new Mock<INotificationMessageGenerator<Email>>();
        emailMessageGenerator.Setup(m => m.Generate()).Returns(emailMsg);

        var emailGeneratorFactory = new Mock<IEmailGeneratorFactory>();
        emailGeneratorFactory.Setup(m => m.CreateAccessCodeEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(emailMessageGenerator.Object);

        string invalidEmail = "testtest.com";
        var command = new RequestEmailAccessCode(invalidEmail);
        var handler = new RequestEmailAccessCodeHandler(accessCodeGenerator.Object, accessCodeStorage.Object, emailNotificationSender.Object, emailGeneratorFactory.Object);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));

        Assert.NotNull(exception);
        Assert.IsType<InvalidEmailException>(exception);
    }

    private static AccessCodeDto CreateAccessCodeDto()
    {
        return new AccessCodeDto() {
            AccessCode = "12345",
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow,
            Expiry = TimeSpan.FromMinutes(15)
        };
    }
}