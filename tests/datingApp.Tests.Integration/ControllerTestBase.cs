using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Integration
{
    public abstract class ControllerTestBase : IClassFixture<OptionsProvider>
    {
        private readonly IAuthenticator _authenticator;
        protected HttpClient Client { get; }

        protected JwtDto Authorize(Guid userId)
        {
            var token = _authenticator.CreateToken(userId);
            return token;
        }

        protected record Error(string Code, string Reason);

        public ControllerTestBase(OptionsProvider optionsProvider)
        {
            var authOptions = optionsProvider.Get<AuthOptions>("auth");
            _authenticator = new Authenticator(new OptionsWrapper<AuthOptions>(authOptions));

            var app = new DatingAppTestApp(ConfigureServices);
            Client = app.Client;
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }
    }
}