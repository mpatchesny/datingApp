using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace datingApp.Infrastructure.Notifications.Views.Emails.AccessCode
{
    public class AccessCodeEmail : PageModel
    {
        private readonly ILogger<AccessCodeEmail> _logger;

        public AccessCodeEmail(ILogger<AccessCodeEmail> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}