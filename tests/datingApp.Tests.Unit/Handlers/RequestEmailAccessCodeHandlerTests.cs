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
    public async Task code_generator_generate_code_method_should_be_called_once()
    {
        var code = CreateAccessCodeDto();
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        _accessCodeStorage.Setup(m => m.Set(code));
        _accessCodeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);
        _emailNotificationSender.Setup(m => m.SendAsync(It.IsAny<Email>()));
        var emailMsg = new Email("sender", "receiver", "subject", "text body", "html body");
        _emailMessageGenerator.Setup(m => m.Generate()).Returns(emailMsg);
        _emailGeneratorFactory.Setup(m => m.CreateAccessCodeEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(_emailMessageGenerator.Object);

        string email = "test@test.com";
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(_accessCodeGenerator.Object, _accessCodeStorage.Object, _emailNotificationSender.Object, _emailGeneratorFactory.Object);
        await handler.HandleAsync(command);
        _accessCodeGenerator.Verify(mock => mock.GenerateCode(email), Times.Once());
    }

    [Fact]
    public async Task code_storage_set_method_should_be_called_once()
    {
        var code = CreateAccessCodeDto();
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        _accessCodeStorage.Setup(m => m.Set(code));
        _accessCodeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);
        _emailNotificationSender.Setup(m => m.SendAsync(It.IsAny<Email>()));
        var emailMsg = new Email("sender", "receiver", "subject", "text body", "html body");
        _emailMessageGenerator.Setup(m => m.Generate()).Returns(emailMsg);
        _emailGeneratorFactory.Setup(m => m.CreateAccessCodeEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(_emailMessageGenerator.Object);

        string email = "test@test.com";
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(_accessCodeGenerator.Object, _accessCodeStorage.Object, _emailNotificationSender.Object, _emailGeneratorFactory.Object);
        await handler.HandleAsync(command);
        _accessCodeStorage.Verify(mock => mock.Set(code), Times.Once());
    }

    [Fact]
    public async Task given_email_address_is_null_throws_NoEmailProvidedException()
    {
        var code = CreateAccessCodeDto();
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        _accessCodeStorage.Setup(m => m.Set(code));
        _accessCodeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);
        _emailNotificationSender.Setup(m => m.SendAsync(It.IsAny<Email>()));
        var emailMsg = new Email("sender", "receiver", "subject", "text body", "html body");
        _emailMessageGenerator.Setup(m => m.Generate()).Returns(emailMsg);
        _emailGeneratorFactory.Setup(m => m.CreateAccessCodeEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(_emailMessageGenerator.Object);

        string email = null;
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(_accessCodeGenerator.Object, _accessCodeStorage.Object, _emailNotificationSender.Object, _emailGeneratorFactory.Object);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<NoEmailProvidedException>(exception);
    }

    [Fact]
    public async Task email_sender_sendasync_method_should_be_called_once()
    {
        var code = CreateAccessCodeDto();
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        _accessCodeStorage.Setup(m => m.Set(code));
        _accessCodeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);
        _emailNotificationSender.Setup(m => m.SendAsync(It.IsAny<Email>()));
        var emailMsg = new Email("sender", "receiver", "subject", "text body", "html body");
        _emailMessageGenerator.Setup(m => m.Generate()).Returns(emailMsg);
        _emailGeneratorFactory.Setup(m => m.CreateAccessCodeEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(_emailMessageGenerator.Object);

        string email = "test@test.com";
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(_accessCodeGenerator.Object, _accessCodeStorage.Object, _emailNotificationSender.Object, _emailGeneratorFactory.Object);
        await handler.HandleAsync(command);
        _emailNotificationSender.Verify(mock => mock.SendAsync(emailMsg), Times.Once());
    }

    [Fact]
    public async Task given_invalid_email_RequestEmailAccessCode_throws_InvalidEmailException()
    {
        var code = CreateAccessCodeDto();
        _accessCodeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        _accessCodeStorage.Setup(m => m.Set(code));
        _accessCodeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);
        _emailNotificationSender.Setup(m => m.SendAsync(It.IsAny<Email>()));
        var emailMsg = new Email("sender", "receiver", "subject", "text body", "html body");
        _emailMessageGenerator.Setup(m => m.Generate()).Returns(emailMsg);
        _emailGeneratorFactory.Setup(m => m.CreateAccessCodeEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(_emailMessageGenerator.Object);

        string invalidEmail = "testtest.com";
        var command = new RequestEmailAccessCode(invalidEmail);
        var handler = new RequestEmailAccessCodeHandler(_accessCodeGenerator.Object, _accessCodeStorage.Object, _emailNotificationSender.Object, _emailGeneratorFactory.Object);
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

    private readonly Mock<IAccessCodeStorage> _accessCodeStorage;
    private readonly Mock<IAccessCodeGenerator> _accessCodeGenerator;
    private readonly  Mock<INotificationSender<Email>> _emailNotificationSender;
    private readonly Mock<IEmailGeneratorFactory> _emailGeneratorFactory;
    private readonly Mock<INotificationMessageGenerator<Email>> _emailMessageGenerator;
    public RequestEmailAccessCodeHandlerTests()
    {
        _accessCodeStorage = new Mock<IAccessCodeStorage>();
        _accessCodeGenerator = new Mock<IAccessCodeGenerator>();
        _emailGeneratorFactory = new Mock<IEmailGeneratorFactory>();
        _emailNotificationSender =  new Mock<INotificationSender<Email>>();
        _emailMessageGenerator = new Mock<INotificationMessageGenerator<Email>>();
    }
}