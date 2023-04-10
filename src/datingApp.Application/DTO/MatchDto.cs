using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class MatchDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public bool IsDisplayed { get; set; }
    public PhotoDto ProfilePicture { get; set; }
    public IEnumerable<MessageDto> Messages { get; set; }
    public DateTime CreatedAt { get; set; }
}