using System.Collections.Generic;
using Azure.Communication.Chat;

namespace ChatModule.Models
{
    public class Thread : IModel
    {
        public string Topic { get; set; }
        public string Id { get; set; }
        public readonly IEnumerable<ChatThreadMember> Members;
        public string InternalKey => Id;
        public string UserKey => Topic;
        public Thread(string topic, string threadId, IEnumerable<ChatThreadMember> members)
        {
            Topic = topic;
            Id = threadId;
            Members = members;
        }
    }
}
