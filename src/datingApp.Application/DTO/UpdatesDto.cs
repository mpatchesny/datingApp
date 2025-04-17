using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class UpdatesDto
{
        public string EntityType { get; set; }
        public string Event { get; set; }
        public object Entity { get; set; }
        public DateTime ChangedAt { get; set; }
};