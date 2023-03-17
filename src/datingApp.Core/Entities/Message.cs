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
        
        [Required(ErrorMessage = "send from required")]
        public User SendFrom { get; private set; }

        [Required(ErrorMessage = "send to required")]
        public User SendTo { get; private set; }

        [Required(ErrorMessage = "message is required")]
        [StringLength(280, ErrorMessage = "message must be maximum 280 characters long")]
        public string Text { get; private set; }

        public bool IsDisplayed { get; private set; }

        [Required(ErrorMessage = "created at is required")]
        public DateTime CreatedAt { get; private set; }

        public Message(User sendFrom, User sendTo, string text, bool isDisplayed, DateTime createdAt)
        {
            SendFrom = sendFrom;
            SendTo = sendTo;
            Text = text;
            IsDisplayed = IsDisplayed;
            CreatedAt = createdAt;
        }

        public void SetAsDisplayed()
        {
            IsDisplayed = true;
        }
    }
}