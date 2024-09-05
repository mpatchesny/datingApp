using System;
using System.Collections.Generic;
using System.Linq;
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
    [Fact (Skip ="FIXME")]
    public async Task given_passed_refresh_token_exists_in_revoked_repository_ResfreshTokenHandler_should_throw_RefreshTokenRevokedException()
    {
        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        await _testDb.DbContext.RevokedRefreshTokens.AddAsync(refreshToken);
        await _testDb.DbContext.SaveChangesAsync();

        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<RefreshTokenRevokedException>(exception);
    }

    [Fact (Skip ="FIXME")]
    public async Task given_passed_refresh_token_not_exists_in_revoked_repository_ResfreshTokenHandler_should_add_passed_token_to_revoked_tokens_repository()
    {
        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        var command = new RefreshJWT(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        var tokenExistsInRepository = await _testDb.DbContext.RevokedRefreshTokens.AnyAsync(x => x.Token == refreshToken.Token);
        Assert.Null(exception);
        Assert.True(tokenExistsInRepository);
    }

    // Arrange
    private readonly RefreshTokenHandler _handler;
    private readonly Mock<ITokenStorage> _tokenStorage;
    private readonly DbRevokedRefreshTokensRepository _revokedRefreshTokensRepository;
    private readonly TestDatabase _testDb;
    public RefreshTokenHandlerTests()
    {
        var expirationTime = DateTime.UtcNow + TimeSpan.FromHours(1);
        var newToken =  new JwtDto
            {
                AccessToken = new TokenDto("access token", expirationTime),
                RefreshToken = new TokenDto("refresh token", expirationTime)
            };
        Mock<IAuthenticator> authenticator = new Mock<IAuthenticator>();
        authenticator.Setup(m => m.CreateToken(It.IsAny<Guid>())).Returns(newToken);
        _tokenStorage = new Mock<ITokenStorage>();
        _tokenStorage.Setup(m => m.Set(It.IsAny<JwtDto>()));

        _testDb = new TestDatabase();
        _revokedRefreshTokensRepository = new DbRevokedRefreshTokensRepository(_testDb.DbContext);
        _handler = new RefreshTokenHandler(authenticator.Object, _tokenStorage.Object, _revokedRefreshTokensRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}