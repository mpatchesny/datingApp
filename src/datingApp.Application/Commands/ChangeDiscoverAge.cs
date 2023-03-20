using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Core.ValueObjects;

namespace datingApp.Application.Commands;

public sealed record ChangeDiscoverAge(int userId, AgeRange age) : ICommand;