using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid SendFromId { get; set; }
    public string Text { get; set; }
    public bool IsDisplayed { get; set; }
    public DateTime CreatedAt { get; set; }
}