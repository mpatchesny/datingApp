using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;

namespace datingApp.Application.Commands;

public sealed record ChangeUser(int UserId, string DateOfBirth=null, string Bio=null, string Job=null) : ICommand;