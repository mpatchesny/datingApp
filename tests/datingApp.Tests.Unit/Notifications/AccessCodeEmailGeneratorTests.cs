using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Notifications;
using datingApp.Infrastructure.Notifications.Generators;
using datingApp.Infrastructure.Notifications.Services;
using datingApp.Infrastructure.Notifications.Views.Emails.AccessCode;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Notifications;

public class AccessCodeEmailGeneratorTests
{
    [Fact]
    public void AccessCodeEmailGenerator_generates_valid_email_with_access_code()
    {
        var recipient = "test@test.com";
        var accessCode = "FOOBAR";
        var expirationTime = TimeSpan.FromMinutes(15);
        var sender = "datingapp@datingapp.com";
        var subject = "subject";
        var textBody = "textbody";
        var htmlBody = "htmlbody";
        var viewName = "/Notifications/Views/Emails/AccessCode/AccessCodeEmail.cshtml";

        var accessCodeEmailViewModel = new AccessCodeEmailViewModel(accessCode, expirationTime);
        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        viewData["EmailSubject"] = subject;
        viewData["EmailTextBody"] = textBody;
        var rendererResult = new Tuple<ViewDataDictionary, string>(viewData, htmlBody);
        var rendererResultAsTask = Task.FromResult<Tuple<ViewDataDictionary, string>>(rendererResult);
        var razorViewToStringRenderer = new Mock<IRazorViewToStringRenderer>();
        razorViewToStringRenderer.Setup(mock => 
            mock.RenderViewToStringAsync(It.IsAny<string>(), It.IsAny<AccessCodeEmailViewModel>()))
            .Returns(rendererResultAsTask);

        var options = Options.Create(new EmailGeneratorOptions { SendFrom = sender });
        var generator = new AccessCodeEmailGenerator(razorViewToStringRenderer.Object, options, recipient, accessCode, expirationTime);

        var email = generator.Generate();
        razorViewToStringRenderer.Verify(mock => mock.RenderViewToStringAsync(viewName, accessCodeEmailViewModel), Times.Once());
        Assert.Equal(recipient, email.Recipient);
        Assert.Equal(sender, email.Sender);
        Assert.Equal(subject, email.Subject);
        Assert.Equal(textBody, email.TextBody);
        Assert.Equal(htmlBody, email.HtmlBody);
    }
}