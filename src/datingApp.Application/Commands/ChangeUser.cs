using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;

namespace datingApp.Application.Commands;

public sealed record ChangeUser(Guid UserId,
                                string DateOfBirth=null,
                                string Bio=null,
                                string Job=null,
                                int? DiscoverAgeFrom=null,
                                int? DiscoverAgeTo=null,
                                int? DiscoverRange=null,
                                int? DiscoverSex=null,
                                double? Lat=null,
                                double? Lon=null) : ICommand;