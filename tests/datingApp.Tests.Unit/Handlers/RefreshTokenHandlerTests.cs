using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class RefreshTokenHandlerTests
{
    [Fact]
    public async Task given_token_is_revoked_RefreshTokenHandler_throws_RefreshTokenRevokedException()
    {
        var authenticator = new Mock<IAuthenticator>();
        var tokenStorage = new Mock<ITokenStorage>();
        var revokedRefreshTokenService = new Mock<IRevokedRefreshTokensService>();
        revokedRefreshTokenService.Setup(x => x.ExistsAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(true));

        var token = "ABCDEF";
        var command = new RefreshJWT(token);
        var handler = new RefreshTokenHandler(authenticator.Object, tokenStorage.Object, revokedRefreshTokenService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<RefreshTokenRevokedException>(exception);
    }

    [Fact]
    public async Task given_token_is_not_revoked_and_is_invalid_RefreshTokenHandler_throws_InvalidRefreshTokenException()
    {
        var claim = CreateClaimsPrincipal(Guid.Empty);
        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(x => x.ValidateRefreshToken(It.IsAny<string>())).Returns(claim);

        var tokenStorage = new Mock<ITokenStorage>();

        var revokedRefreshTokenService = new Mock<IRevokedRefreshTokensService>();
        revokedRefreshTokenService.Setup(x => x.ExistsAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(false));

        var token = "ABCDEF";
        var command = new RefreshJWT(token);
        var handler = new RefreshTokenHandler(authenticator.Object, tokenStorage.Object, revokedRefreshTokenService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidRefreshTokenException>(exception);
    }

    [Fact]
    public async Task given_token_is_not_revoked_and_is_valid_RefreshTokenHandler_should_succeed()
    {
        var userId = Guid.NewGuid();
        var claim = CreateClaimsPrincipal(userId);
        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(x => x.ValidateRefreshToken(It.IsAny<string>())).Returns(claim);

        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(x => x.Set(It.IsAny<JwtDto>()));

        var revokedRefreshTokenService = new Mock<IRevokedRefreshTokensService>();
        revokedRefreshTokenService.Setup(x => x.ExistsAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(false));

        var token = "ABCDEF";
        var command = new RefreshJWT(token);
        var handler = new RefreshTokenHandler(authenticator.Object, tokenStorage.Object, revokedRefreshTokenService.Object);

        await handler.HandleAsync(command);
        revokedRefreshTokenService.Verify(x => x.ExistsAsync(token), Times.Once());
        revokedRefreshTokenService.Verify(x => x.AddAsync(It.IsAny<TokenDto>()), Times.Once());
        authenticator.Verify(x => x.ValidateRefreshToken(token), Times.Once());
        authenticator.Verify(x => x.CreateToken(userId), Times.Once());
        tokenStorage.Verify(x => x.Set(It.IsAny<JwtDto>()), Times.Once());
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(Guid userId)
    {
        var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            };
        var identity = new ClaimsIdentity(claims, authenticationType: string.Empty, nameType: JwtRegisteredClaimNames.Sub, roleType: string.Empty);
        return new ClaimsPrincipal(identity);
    }
}