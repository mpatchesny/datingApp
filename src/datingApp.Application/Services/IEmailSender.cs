using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IEmailSender
{
    public Task SendAsync(string receiver, string subject, string body);
}