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
    public async Task given_refresh_token_exists_in_revoked_repository_ResfreshTokenHandler_should_throw_RefreshTokenRevokedException()
    {
        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(new JwtDto());
        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));
        var handler = new RefreshJWTHandler(authenticator.Object, tokenStorage.Object, _revokedRefreshTokensRepository);

        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        await _testDb.DbContext.RevokedRefreshTokens.AddAsync(refreshToken);
        await _testDb.DbContext.SaveChangesAsync();

        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<RefreshTokenRevokedException>(exception);
    }

    [Fact]
    public async Task given_refresh_token_validation_failed_ResfreshTokenHandler_should_throw_InvalidRefreshTokenException()
    {
        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(new JwtDto());
        authenticator.Setup(m => m.ValidateRefreshToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());
        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));
        var handler = new RefreshJWTHandler(authenticator.Object, tokenStorage.Object, _revokedRefreshTokensRepository);

        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));

        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidRefreshTokenException>(exception);
    }

    [Fact]
    public async Task given_refresh_token_succeed_ResfreshTokenHandler_should_add_new_JwtDto_to_token_storage()
    {
        var expirationTime = DateTime.UtcNow + TimeSpan.FromHours(1);
        var newToken =  new JwtDto
            {
                AccessToken = new TokenDto("access token", expirationTime),
                RefreshToken = new TokenDto("refresh token", expirationTime)
            };
        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(newToken);
        var userId = Guid.NewGuid();
        var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            }));
        authenticator.Setup(m => m.ValidateRefreshToken(It.IsAny<string>())).Returns(claimPrincipal);
        
        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));
        var handler = new RefreshJWTHandler(authenticator.Object, tokenStorage.Object, _revokedRefreshTokensRepository);

        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.Null(exception);
        tokenStorage.Verify(x => x.Set(newToken), Times.Once);
    }

    [Fact]
    public async Task given_refresh_token_succeed_ResfreshTokenHandler_should_add_refresh_token_to_revoked_tokens_repository()
    {
        var expirationTime = DateTime.UtcNow + TimeSpan.FromHours(1);
        var newToken =  new JwtDto
            {
                AccessToken = new TokenDto("access token", expirationTime),
                RefreshToken = new TokenDto("refresh token", expirationTime)
            };
        var authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(newToken);
        var userId = Guid.NewGuid();
        var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>
        
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            }));
        authenticator.Setup(m => m.ValidateRefreshToken(It.IsAny<string>())).Returns(claimPrincipal);
        
        var tokenStorage = new Mock<ITokenStorage>();
        tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));
        var handler = new RefreshJWTHandler(authenticator.Object, tokenStorage.Object, _revokedRefreshTokensRepository);

        var refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        var tokenExistsInRepository = await _testDb.DbContext.RevokedRefreshTokens.AnyAsync(x => x.Token == refreshToken.Token);
        Assert.Null(exception);
        Assert.True(tokenExistsInRepository);
    }

    // Arrange
    private readonly DbRevokedRefreshTokensRepository _revokedRefreshTokensRepository;
    private readonly TestDatabase _testDb;
    public RefreshTokenHandlerTests()
    {
        _testDb = new TestDatabase();
        _revokedRefreshTokensRepository = new DbRevokedRefreshTokensRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}