using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Storage;

namespace datingApp.Infrastructure.Storage;

internal sealed class HttpContextIsLikedByOtherUserStorage : IIsLikedByOtherUserStorage
{
    private const string _key = "is_liked_by_other_user";
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpContextIsLikedByOtherUserStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IsLikedByOtherUserDto Get()
    {
        return _httpContextAccessor.HttpContext?.Items.TryGetValue(_key, out var isLikedByOtherUser) == true
            ? isLikedByOtherUser as IsLikedByOtherUserDto
            : null;
    }

    public void Set(IsLikedByOtherUserDto isLikedByOtherUser)
    {
        _httpContextAccessor.HttpContext?.Items.TryAdd(_key, isLikedByOtherUser);
    }
}