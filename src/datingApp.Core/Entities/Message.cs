using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class Message
    {
        public long Id { get; }
        
        [Required]
        public User SendFrom { get; private set; }

        [Required]
        public User SendTo { get; private set; }

        [Required]
        [StringLength(280, ErrorMessage = "message must be maximum 280 characters long")]
        public string Text {get; private set; }

        [Required]
        public DateTime CreatedAt { get; private set; }

        public Message(User sendFrom, User sendTo, string text, DateTime createdAt)
        {
            SendFrom = sendFrom;
            SendTo = sendTo;
            Text = text;
            CreatedAt = createdAt;
        }
    }
}