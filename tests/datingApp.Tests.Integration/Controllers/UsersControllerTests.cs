using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using Xunit;

namespace datingApp.Tests.Integration.Controllers
{
    public class UsersControllerTests : ControllerTestBase
    {
        [Fact]
        public async Task given_valid_sign_up_request_post_should_return_created_201_status_code()
        {
            var command = new SignUp(Guid.Empty, "123456789", "test@test.com", "Janusz", "2000-01-01", 1, 1);
            var response = await Client.PostAsJsonAsync("users", command);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers);
        }

        [Fact]
        public async Task get_public_user_endpoint_when_not_logged_in_should_return_401_unauthorized()
        {
            var guid = new Guid("00000000-0000-0000-0000-000000000001");
            var response = await Client.GetAsync($"users/{guid}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}