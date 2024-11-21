using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;

namespace datingApp.Application.Commands.Handlers;

public sealed record AddPhoto2(Guid PhotoId, Guid UserId, Stream PhotoStream) : AuthenticatedCommandBase;