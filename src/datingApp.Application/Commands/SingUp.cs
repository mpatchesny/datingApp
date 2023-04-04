using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Core.Entities;

namespace datingApp.Application.Commands;

public sealed record SingUp(string Phone, string Email, string Name, DateOnly DateOfBirth, int Sex, int DiscoverSex, string Job="", string Bio="") : ICommand;