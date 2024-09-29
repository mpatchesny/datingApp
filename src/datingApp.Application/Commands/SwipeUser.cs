using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Core.Entities;

namespace datingApp.Application.Commands;

public sealed record SwipeUser(Guid SwipedById, Guid SwipedWhoId, int Like) : AuthenticatedCommandBase;