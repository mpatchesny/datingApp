using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IEmailGenerator
{
    public Email Generate(string ReceiverEmailAddress, Dictionary<string, string> kwargs);
}