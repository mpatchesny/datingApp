using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Core.Entities;

namespace datingApp.Application.Commands;

public sealed record SwipeUser(int SwipedById, int SwipedWhoId, int Like, DateTime swipeTime) : ICommand;