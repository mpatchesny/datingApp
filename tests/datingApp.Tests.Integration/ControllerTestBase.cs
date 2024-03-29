using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace datingApp.Tests.Integration
{
    public abstract class ControllerTestBase : IClassFixture<OptionsProvider>
    {
        protected HttpClient Client { get; }

        public ControllerTestBase(OptionsProvider optionsProvider)
        {
            var app = new DatingAppTestApp();
            Client = app.Client;
        }
    }
}