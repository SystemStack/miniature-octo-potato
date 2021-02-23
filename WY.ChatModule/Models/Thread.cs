using Azure.Communication.Chat;

namespace ChatModule.Models
{
    public class Thread : IModel
    {
        public string InternalKey => Id;
        public string UserKey => Topic;
        public string Id { get; private set; }
        public string Topic { get; private set; }
        public ChatThread ChatThread { get; private set; }
        public Thread(ChatThread thread)
        {
            ChatThread = thread;
            Id = thread.Id;
            Topic = thread.Topic;
        }
    }
}
