using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities
{
    public class Message
    {
        public int Id { get; }
        public int SendFromId { get; private set; }
        public int SendToId { get; private set; }
        // [StringLength(280, ErrorMessage = "message must be maximum 280 characters long")]
        public string Text { get; private set; }
        public bool IsDisplayed { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Message(int id, int sendFromId, int sendToId, string text, bool isDisplayed, DateTime createdAt)
        {
            Id = id;
            SendFromId = sendFromId;
            SendToId = sendToId;
            Text = text;
            IsDisplayed = IsDisplayed;
            CreatedAt = createdAt;
        }

        public void SetDisplayed()
        {
            IsDisplayed = true;
        }
    }
}