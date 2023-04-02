using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class MessageDto
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public int SendFromId { get; set; }
    public string Text { get; set; }
    public bool IsDisplayed { get; set; }
    public DateTime CreatedAt { get; set; }
}