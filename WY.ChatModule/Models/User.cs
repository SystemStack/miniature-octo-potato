using Azure.Communication;
using Azure.Communication.Administration.Models;
using Azure.Communication.Chat;
using System.Text.Json.Serialization;

namespace ChatModule.Models
{
    public class User : IModel
    {
        public string UserId { get; set; }
        public string Id => CommunicationUser.Id;
        
        [JsonIgnore]
        public string InternalKey => Id;
        [JsonIgnore]
        public string UserKey => UserId;

        internal CommunicationUser CommunicationUser { get; private set; }
        internal CommunicationUserToken CommunicationUserToken { get; private set; }
        internal ChatThreadMember ChatThreadMember { get; private set; }
        public User(string userId, CommunicationUser communicationUser)
        {
            UserId = userId;
            CommunicationUser = communicationUser;
        }
        public User(string userId, CommunicationUserToken token)
            : this(userId, token.User)
        {
            CommunicationUserToken = token;
        }

        public User(string userId, ChatThreadMember member)
            : this(userId, member.User)
        {
            ChatThreadMember = member;
        }
    }
}
