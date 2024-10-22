using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Notifications.Views.Emails.AccessCode;

internal sealed record AccessCodeEmailViewModel(string AccessCode, TimeSpan ExpirationTime);