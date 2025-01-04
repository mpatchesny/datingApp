using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Storage;
using datingApp.Core.Consts;
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
    public async void given_swipe_not_exists_SwipeUserHanlde_adds_new_swipe()
    {
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(x => x.GetByIdAsync(It.IsAny<MatchId>())).Returns(Task.FromResult<Match>(null));
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()));
        
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBySwipedWho(It.IsAny<UserId>(), It.IsAny<UserId>())).Returns(Task.FromResult<List<Swipe>>(new List<Swipe>()));
        var addedSwipe = (Swipe) null;
        swipeRepository.Setup(x => x.AddAsync(It.IsAny<Swipe>()))
            .Callback<Swipe>(s => addedSwipe = s);

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        var like = 1;
        var command = new SwipeUser(Guid.NewGuid(), Guid.NewGuid(), like);
        var handler = new SwipeUserHandler(swipeRepository.Object, matchRepository.Object, isLikedByOtherUserStorage.Object);
        await handler.HandleAsync(command);

        swipeRepository.Verify(x => x.AddAsync(addedSwipe), Times.Once());
        isLikedByOtherUserStorage.Verify(x => x.Set(isLikedByOtherUser), Times.Once());
        Assert.Equal(like, (int) addedSwipe.Like);
        Assert.Equal(command.SwipedById, addedSwipe.SwipedById.Value);
        Assert.Equal(command.SwipedWhoId, addedSwipe.SwipedWhoId.Value);
    }

    [Fact]
    public async void given_swipe_exists_SwipeUserHanlde_not_adds_new_swipe()
    {
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()));
       
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var swipe = new Swipe(userId1, userId2, Like.Like, DateTime.UtcNow);
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBySwipedWho(It.IsAny<UserId>(), It.IsAny<UserId>())).Returns(Task.FromResult<List<Swipe>>(new List<Swipe>(){ swipe }));
        var addedSwipe = (Swipe) null;
        swipeRepository.Setup(x => x.AddAsync(It.IsAny<Swipe>()))
            .Callback<Swipe>(s => addedSwipe = s);

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        var like = (int) Like.Like;
        var command = new SwipeUser(userId1, userId2, like);
        var handler = new SwipeUserHandler(swipeRepository.Object, matchRepository.Object, isLikedByOtherUserStorage.Object);
        await handler.HandleAsync(command);

        swipeRepository.Verify(x => x.AddAsync(addedSwipe), Times.Never());
        isLikedByOtherUserStorage.Verify(x => x.Set(isLikedByOtherUser), Times.Once());
    }

    [Fact]
    public async void given_other_user_swipe_not_exists_SwipeUserHanlder_returns_false_is_liked_by_other_user()
    {
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()));

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var swipe = new Swipe(userId1, userId2, Like.Like, DateTime.UtcNow);
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBySwipedWho(It.IsAny<UserId>(), It.IsAny<UserId>())).Returns(Task.FromResult<List<Swipe>>(new List<Swipe>(){ swipe }));

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        var like = 1;
        var command = new SwipeUser(userId1, userId2, like);
        var handler = new SwipeUserHandler(swipeRepository.Object, matchRepository.Object, isLikedByOtherUserStorage.Object);
        await handler.HandleAsync(command);

        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
        isLikedByOtherUserStorage.Verify(x => x.Set(isLikedByOtherUser), Times.Once());
    }

    [Fact]
    public async void given_other_user_not_liked_user_SwipeUserHanlder_returns_false_is_liked_by_other_user()
    {
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()));

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var swipe = new Swipe(userId1, userId2, Like.Like, DateTime.UtcNow);
        var otherUserSwipe = new Swipe(userId2, userId1, Like.Pass, DateTime.UtcNow);
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBySwipedWho(It.IsAny<UserId>(), It.IsAny<UserId>())).Returns(Task.FromResult<List<Swipe>>(new List<Swipe>(){ swipe, otherUserSwipe }));

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        var like = (int) Like.Like;
        var command = new SwipeUser(userId1, userId2, like);
        var handler = new SwipeUserHandler(swipeRepository.Object, matchRepository.Object, isLikedByOtherUserStorage.Object);
        await handler.HandleAsync(command);

        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
        isLikedByOtherUserStorage.Verify(x => x.Set(isLikedByOtherUser), Times.Once());
    }

    [Fact]
    public async void given_other_user_liked_user_SwipeUserHanlder_returns_true_is_liked_by_other_user()
    {
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()));

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var swipe = new Swipe(userId1, userId2, Like.Like, DateTime.UtcNow);
        var otherUserSwipe = new Swipe(userId2, userId1, Like.Like, DateTime.UtcNow);
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBySwipedWho(It.IsAny<UserId>(), It.IsAny<UserId>())).Returns(Task.FromResult<List<Swipe>>(new List<Swipe>(){ swipe, otherUserSwipe }));

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        var like = (int) Like.Like;
        var command = new SwipeUser(userId1, userId2, like);
        var handler = new SwipeUserHandler(swipeRepository.Object, matchRepository.Object, isLikedByOtherUserStorage.Object);
        await handler.HandleAsync(command);

        Assert.True(isLikedByOtherUser.IsLikedByOtherUser);
        isLikedByOtherUserStorage.Verify(x => x.Set(isLikedByOtherUser), Times.Once());
    }

    [Fact]
    public async void given_other_user_liked_user_SwipeUserHanlder_returns_true_is_liked_by_other_user_2()
    {
        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()));

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var swipe = new Swipe(userId1, userId2, Like.Like, DateTime.UtcNow);
        var otherUserSwipe = new Swipe(userId2, userId1, Like.Like, DateTime.UtcNow);
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBySwipedWho(It.IsAny<UserId>(), It.IsAny<UserId>())).Returns(Task.FromResult<List<Swipe>>(new List<Swipe>(){ swipe, otherUserSwipe }));

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        var like = (int) Like.Like;
        var command = new SwipeUser(userId1, userId2, like);
        var handler = new SwipeUserHandler(swipeRepository.Object, matchRepository.Object, isLikedByOtherUserStorage.Object);
        await handler.HandleAsync(command);

        Assert.True(isLikedByOtherUser.IsLikedByOtherUser);
        isLikedByOtherUserStorage.Verify(x => x.Set(isLikedByOtherUser), Times.Once());
    }

    [Fact]
    public async void given_two_users_liked_each_ohter_SwipeUserHanlder_adds_new_match_1()
    {
        var matchRepository = new Mock<IMatchRepository>();
        Match match = null;
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()))
            .Callback<Match>(s => match = s);

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var otherUserSwipe = new Swipe(userId2, userId1, Like.Like, DateTime.UtcNow);
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBySwipedWho(It.IsAny<UserId>(), It.IsAny<UserId>())).Returns(Task.FromResult<List<Swipe>>(new List<Swipe>(){ otherUserSwipe }));

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        var like = (int) Like.Like;
        var command = new SwipeUser(userId1, userId2, like);
        var handler = new SwipeUserHandler(swipeRepository.Object, matchRepository.Object, isLikedByOtherUserStorage.Object);
        await handler.HandleAsync(command);

        matchRepository.Verify(x => x.AddAsync(match), Times.Once());
        Assert.Equal(userId1, match.UserId1.Value);
        Assert.Equal(userId2, match.UserId2.Value);
        isLikedByOtherUserStorage.Verify(x => x.Set(isLikedByOtherUser), Times.Once());
    }
    
    [Fact]
    public async void given_two_users_liked_each_ohter_SwipeUserHanlder_adds_new_match_2()
    {
        var matchRepository = new Mock<IMatchRepository>();
        Match match = null;
        matchRepository.Setup(x => x.AddAsync(It.IsAny<Match>()))
            .Callback<Match>(s => match = s);

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var swipe =new Swipe(userId1, userId2, Like.Like, DateTime.UtcNow);
        var otherUserSwipe = new Swipe(userId2, userId1, Like.Like, DateTime.UtcNow);
        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.GetBySwipedBySwipedWho(It.IsAny<UserId>(), It.IsAny<UserId>()))
            .Returns(Task.FromResult<List<Swipe>>(new List<Swipe>(){ swipe, otherUserSwipe }));

        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);

        var like = (int) Like.Like;
        var command = new SwipeUser(userId1, userId2, like);
        var handler = new SwipeUserHandler(swipeRepository.Object, matchRepository.Object, isLikedByOtherUserStorage.Object);
        await handler.HandleAsync(command);

        matchRepository.Verify(x => x.AddAsync(match), Times.Once());
        Assert.Equal(userId1, match.UserId1.Value);
        Assert.Equal(userId2, match.UserId2.Value);
        isLikedByOtherUserStorage.Verify(x => x.Set(isLikedByOtherUser), Times.Once());
    }
}