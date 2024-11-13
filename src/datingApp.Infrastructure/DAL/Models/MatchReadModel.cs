using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Models;

internal class MatchReadModel
{
    public Guid Id { get; set; }
    public UserReadModel Owner { get; set; }
    public UserReadModel User { get; set; }
    public bool IsDisplayed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastChangeTime { get; set; }
    public IEnumerable<MessageReadModel> Messages { get; set; }
}