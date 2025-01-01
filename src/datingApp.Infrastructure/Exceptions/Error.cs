using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Exceptions;

internal sealed record Error(string Code, string Reason);