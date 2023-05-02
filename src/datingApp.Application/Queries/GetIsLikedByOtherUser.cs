using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetIsLikedByOtherUser : AuthenticatedQueryBase<IsLikedByOtherUserDto>
{
    public Guid SwipedById { get; set; }
    public Guid SwipedWhoId { get; set; }
}