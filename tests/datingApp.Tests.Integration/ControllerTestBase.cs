using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Tests.Integration
{
    public abstract class ControllerTestBase
    {
        protected HttpClient Client { get; }

        public ControllerTestBase()
        {
            var app = new DatingAppTestApp();
            Client = app.Client;
        }
    }
}