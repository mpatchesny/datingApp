using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Notifications;

public interface IEmailGeneratorFactory
{
    INotificationMessageGenerator<Email> CreateAccessCodeEmail(string recipient, string accessCode, TimeSpan expirationTime);
}