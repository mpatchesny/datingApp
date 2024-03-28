using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace datingApp.Tests.Integration.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task get_public_user_endpoint_when_not_logged_in_should_return_401_unauthorized()
        {
            var app = new DatingAppTestApp();
            var guid = new Guid("00000000-0000-0000-0000-000000000001");
            var response = await app.Client.GetAsync($"/users/{guid}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}