using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;

namespace datingApp.Application.Commands;

public sealed record SetMatchAsDisplayed(Guid MatchId, Guid DisplayedByUserId) : AuthenticatedCommandBase;