using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Security.Authorization;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.Security;

public class UserPermissionHandlerTests
{
    [Fact (Skip = "can't test internal sealed class with protected method")]
    public async Task given_resource_id_equals_context_user_identity_name_handler_authorization_succeedAsync()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.MaleAndFemale, 18, 100, 100, 45.5, 45.5);
        var resource = new User(settings.UserId, "012345689", "test@test.com", "Test", new DateOnly(1990, 1, 1), UserSex.Male, null, settings);
        // var result = await _handler.HandleRequirementAsync(_authorizationHandlerContext.Object, new IsOwnerRequirement(), resource);
        // Assert.Equal(AuthorizationResult.Success, result);
    }

    [Fact (Skip = "can't test internal sealed class with protected method")]
    public async void given_resource_id_not_equals_context_user_identity_name_handler_authorization_fails()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.MaleAndFemale, 18, 100, 100, 45.5, 45.5);
        var resource = new User(settings.UserId, "012345689", "test@test.com", "Test", new DateOnly(1990, 1, 1), UserSex.Male, null, settings);
        // var result = await _handler.HandleRequirementAsync(_authorizationHandlerContext.Object, new IsOwnerRequirement(), resource);
        // Assert.Equal(AuthorizationResult.Failed, result);
    }

    private readonly Mock<AuthorizationHandlerContext> _authorizationHandlerContext;
    private readonly UserPermissionHandler _handler;
    public UserPermissionHandlerTests()
    {
        _handler = new UserPermissionHandler();
        _authorizationHandlerContext = new Mock<AuthorizationHandlerContext>();
        // TODO: auth handler context moq return value
    }
}