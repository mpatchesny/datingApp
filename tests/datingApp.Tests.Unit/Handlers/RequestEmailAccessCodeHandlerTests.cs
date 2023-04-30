using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using datingApp.Application.Services;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class RequestEmailAccessCodeHandlerTests
{
    [Fact]
    public async Task code_generator_generate_code_method_should_be_called_once()
    {
        var code = new AccessCodeDto() {
            AccessCode ="12345",
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow,
            Expiry = TimeSpan.FromMinutes(15)
        };

        var codeStorage = new Mock<IAccessCodeStorage>();
        codeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        codeStorage.Setup(m => m.Set(code));

        var codeGenerator = new Mock<IAccessCodeGenerator>();
        codeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);

        var emailSender = new Mock<IEmailSender>();
        emailSender.Setup(m => m.SendAsync(It.IsAny<Email>()));

        var emailMsg = new Email();
        var emailGenerator = new Mock<IEmailGenerator>();
        emailGenerator.Setup(m => m.Generate(It.IsAny<string>(), It.IsAny<Dictionary<string,string>>())).Returns(emailMsg);

        string email = "test@test.com";
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(codeGenerator.Object, codeStorage.Object, emailSender.Object, emailGenerator.Object);
        await handler.HandleAsync(command);

        codeGenerator.Verify(mock => mock.GenerateCode(email), Times.Once());
    }

    [Fact]
    public async Task code_storage_set_method_should_be_called_once()
    {
        var code = new AccessCodeDto() {
            AccessCode ="12345",
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow,
            Expiry = TimeSpan.FromMinutes(15)
        };

        var codeStorage = new Mock<IAccessCodeStorage>();
        codeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        codeStorage.Setup(m => m.Set(code));

        var codeGenerator = new Mock<IAccessCodeGenerator>();
        codeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);

        var emailSender = new Mock<IEmailSender>();
        emailSender.Setup(m => m.SendAsync(It.IsAny<Email>()));

        var emailMsg = new Email();
        var emailGenerator = new Mock<IEmailGenerator>();
        emailGenerator.Setup(m => m.Generate(It.IsAny<string>(), It.IsAny<Dictionary<string,string>>())).Returns(emailMsg);

        string email = "test@test.com";
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(codeGenerator.Object, codeStorage.Object, emailSender.Object, emailGenerator.Object);
        await handler.HandleAsync(command);

        codeStorage.Verify(mock => mock.Set(code), Times.Once());
    }

    [Fact]
    public async Task email_sender_sendasync_method_should_be_called_once()
    {
        var code = new AccessCodeDto() {
            AccessCode ="12345",
            EmailOrPhone = "test@test.com",
            ExpirationTime = DateTime.UtcNow,
            Expiry = TimeSpan.FromMinutes(15)
        };

        var codeStorage = new Mock<IAccessCodeStorage>();
        codeStorage.Setup(m => m.Get(It.IsAny<string>())).Returns(code);
        codeStorage.Setup(m => m.Set(code));

        var codeGenerator = new Mock<IAccessCodeGenerator>();
        codeGenerator.Setup(m => m.GenerateCode(It.IsAny<string>())).Returns(code);

        var emailSender = new Mock<IEmailSender>();
        emailSender.Setup(m => m.SendAsync(It.IsAny<Email>()));

        var emailMsg = new Email();
        var emailGenerator = new Mock<IEmailGenerator>();
        emailGenerator.Setup(m => m.Generate(It.IsAny<string>(), It.IsAny<Dictionary<string,string>>())).Returns(emailMsg);

        string email = "test@test.com";
        var command = new RequestEmailAccessCode(email);
        var handler = new RequestEmailAccessCodeHandler(codeGenerator.Object, codeStorage.Object, emailSender.Object, emailGenerator.Object);
        await handler.HandleAsync(command);

        emailSender.Verify(mock => mock.SendAsync(It.IsAny<Email>()), Times.Once());
    }
}