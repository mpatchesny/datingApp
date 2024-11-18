using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class RefreshTokenHandlerTests : IDisposable
{
    [Fact]
    public async Task given_refresh_token_exists_in_revoked_repository_ResfreshTokenHandler_throws_RefreshTokenRevokedException()
    {
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(new JwtDto());
        _tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));

        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        await _testDb.DbContext.RevokedRefreshTokens.AddAsync(refreshToken);
        await _testDb.DbContext.SaveChangesAsync();

        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<RefreshTokenRevokedException>(exception);
    }

    [Fact]
    public async Task given_refresh_token_validation_failed_ResfreshTokenHandler_throws_InvalidRefreshTokenException()
    {
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(new JwtDto());
        _authenticator.Setup(m => m.ValidateRefreshToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());
        _tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));

        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidRefreshTokenException>(exception);
    }

    [Fact]
    public async Task given_refresh_token_succeed_ResfreshTokenHandler_should_add_new_JwtDto_to_token_storage()
    {
        var expirationTime = DateTime.UtcNow + TimeSpan.FromHours(1);
        var newToken = CreateToken(expirationTime);
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(newToken);

        var claimPrincipal = CreateClaimsPrincipal();
        _authenticator.Setup(m => m.ValidateRefreshToken(It.IsAny<string>())).Returns(claimPrincipal);
        _tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));

        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        _tokenStorage.Verify(x => x.Set(newToken), Times.Once);
    }

    [Fact]
    public async Task given_refresh_token_succeed_ResfreshTokenHandler_should_add_passed_refresh_token_to_revoked_tokens_repository()
    {
        var expirationTime = DateTime.UtcNow + TimeSpan.FromHours(1);
        var newToken = CreateToken(expirationTime);
        _authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(newToken);

        var claimPrincipal = CreateClaimsPrincipal();
        _authenticator.Setup(m => m.ValidateRefreshToken(It.IsAny<string>())).Returns(claimPrincipal);
        _tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));

        var refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        var tokenExistsInRepository = await _testDb.DbContext.RevokedRefreshTokens.AnyAsync(x => x.Token == refreshToken.Token);
        Assert.Null(exception);
        Assert.True(tokenExistsInRepository);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly RefreshJWTHandler _handler;
    private readonly Mock<ITokenStorage> _tokenStorage;
    private readonly Mock<IAuthenticator> _authenticator;
    private readonly RevokedRefreshTokensService _revokedRefreshTokensService;
    public RefreshTokenHandlerTests()
    {
        _testDb = new TestDatabase();
        _revokedRefreshTokensService = new RevokedRefreshTokensService(_testDb.DbContext);
        _tokenStorage = new Mock<ITokenStorage>();
        _authenticator = new Mock<IAuthenticator>();
        _handler = new RefreshJWTHandler(_authenticator.Object, _tokenStorage.Object, _revokedRefreshTokensService);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }

    private static ClaimsPrincipal CreateClaimsPrincipal()
    {
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            };
        var identity = new ClaimsIdentity(claims, authenticationType: string.Empty, nameType: JwtRegisteredClaimNames.Sub, roleType: string.Empty);
        return new ClaimsPrincipal(identity);
    }

    private static JwtDto CreateToken(DateTime expirationTime)
    {
        return new JwtDto
            {
                AccessToken = new TokenDto("access token", expirationTime),
                RefreshToken = new TokenDto("refresh token", expirationTime)
            };
    }
}