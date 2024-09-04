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
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class RefreshTokenHandlerTests : IDisposable
{
    [Fact]
    public async Task given_passed_refresh_token_exists_in_revoked_repository_ResfreshTokenHandler_should_throw_RefreshTokenRevokedException()
    {
        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        await _revokedRefreshTokensRepository.AddAsync(refreshToken);

        var command = new RefreshToken(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<RefreshTokenRevokedException>(exception);
    }

    [Fact]
    public async Task given_passed_refresh_token_not_exists_in_revoked_repository_ResfreshTokenHandler_should_generate_new_access_refresh_token_pair_and_add_it_to_token_storage()
    {
        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        var command = new RefreshToken(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        JwtDto newToken = null;
        _tokenStorage.Verify(mock => mock.Set(newToken), Times.Once());
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_passed_refresh_token_not_exists_in_revoked_repository_ResfreshTokenHandler_should_add_passed_token_to_revoked_tokens_repository()
    {
        TokenDto refreshToken = new TokenDto("abc", DateTime.UtcNow + TimeSpan.FromDays(1));
        var command = new RefreshToken(refreshToken.Token);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        var tokenExistsInRepository = await _revokedRefreshTokensRepository.ExistsAsync(refreshToken.Token);
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
        // TODO: mocks
        // TODO: authenticator returns specific JwtDto
        Mock<IAuthenticator> authenticator = null;
        _tokenStorage = new Mock<ITokenStorage>();
        // _tokenStorage.Setup(m => m.Set(It.Is<TokenDto>())));
        // _tokenStorage.Setup(m => m.Get(It.Is(newToken)));

        _revokedRefreshTokensRepository = new DbRevokedRefreshTokensRepository(_testDb.DbContext);
        _handler = new RefreshTokenHandler(authenticator.Object, _tokenStorage.Object, _revokedRefreshTokensRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}