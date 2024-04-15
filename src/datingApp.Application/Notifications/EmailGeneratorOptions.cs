using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Notifications;

public sealed class EmailGeneratorOptions
{
    public string SubjectTemplate { get; set; }
    public string BodyTemplate { get; set; }
}