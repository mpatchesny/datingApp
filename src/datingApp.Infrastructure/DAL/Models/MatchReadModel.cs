using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Models;

internal class MatchReadModel
{
    public Guid Id { get; set; }
    public UserReadModel User1 { get; set; }
    public UserReadModel User2 { get; set; }
    public bool IsDisplayedByUser1 { get; set; }
    public bool IsDisplayedByUser2 { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastChangeTime { get; set; }
    public IEnumerable<MessageReadModel> Messages { get; set; }
}