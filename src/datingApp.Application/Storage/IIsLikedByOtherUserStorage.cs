using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;

namespace datingApp.Application.Storage;

public interface IIsLikedByOtherUserStorage
{
    public IsLikedByOtherUserDto Get();
    public void Set(IsLikedByOtherUserDto isLikedByOtherUser);
}