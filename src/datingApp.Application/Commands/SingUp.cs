using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Core.Entities;

namespace datingApp.Application.Commands;

public sealed record SingUp(string phone, string email, string name, DateOnly dateOfBirth, int sex, int discoverSex, string job="", string bio="") : ICommand;