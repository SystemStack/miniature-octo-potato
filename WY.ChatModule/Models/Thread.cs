using System.Collections.Generic;
using Azure.Communication.Chat;

namespace ChatModule.Models
{
    public class Thread : IModel
    {
        public string InternalKey => Id;
        public string UserKey => Topic;
        public string Id { get; private set; }
        public string Topic { get; private set; }
        public IEnumerable<User> Members { get; }
        public IEnumerable<ChatMessage> Messages { get; }

        public Thread(string id, string topic, IEnumerable<User> members = null, IEnumerable<ChatMessage> messages = null)
        {
            Id = id;
            Topic = topic;
            Members = members ?? new List<User>();
            Messages = messages ?? new List<ChatMessage>();
        }
    }
}
