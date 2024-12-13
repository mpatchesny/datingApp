using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Storage;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using Moq;
using Org.BouncyCastle.Crypto.Engines;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Unit.Handlers;

public class SwipeUserHandlerTests
{
    [Fact]
    public void Test1()
    {
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>())).Returns(Task.FromResult<Match>(null));
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()));
        
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBy(It.IsAny<UserId>(), It.IsAny<UserId>())).Returns(Task.FromResult<List<Swipe>>(new List<Swipe>()));
        var addedSwipe = (Swipe) null;
        swipeRepository.Setup(x => x.AddAsync(It.IsAny<Swipe>()))
            .Callback<Swipe>(s => addedSwipe = s);

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        Assert.True(true);
    }
}