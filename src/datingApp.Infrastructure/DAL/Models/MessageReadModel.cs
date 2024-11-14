using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Models;

internal sealed class MessageReadModel
{
    public Guid Id { get; set; }
    public MatchReadModel Match { get; set; }
    public UserReadModel SendFrom { get; set; }
    public string Text { get; set; }
    public bool IsDisplayed { get; set; }
    public DateTime CreatedAt { get; set; }
}