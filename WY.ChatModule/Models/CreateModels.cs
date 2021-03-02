using Azure.Communication.Chat;
using System.Collections.Generic;

namespace ChatModule.Models
{
    public interface ICreationModel
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
    }

    public class BaseCreationModel : ICreationModel
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
    }

    public class ThreadCreationModel : BaseCreationModel
    {
        public string Topic { get; set; }
        public List<string> Members { get; set; }
        public ThreadCreationModel() { }
    }

    public class MessageCreationModel : BaseCreationModel
    {
        public string Topic { get; set; }
        public string Content { get; set; }
        public ChatMessagePriority Priority { get; set; }
        public MessageCreationModel() { }
    }
}
