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
    private readonly HttpContextAccessor _httpContextAccessor;
    public HttpContextIsLikedByOtherUserStorage(HttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IsLikedByOtherUserDto Get()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return null;
        }

        if (_httpContextAccessor.HttpContext.Items.TryGetValue(_key, out var jwt))
        {
            return jwt as IsLikedByOtherUserDto;
        }

        return null;
    }

    public void Set(IsLikedByOtherUserDto isLikedByOtherUser)
    {
        _httpContextAccessor.HttpContext?.Items.TryAdd(_key, isLikedByOtherUser);
    }
}