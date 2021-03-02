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

        // [JsonIgnore]
        public string InternalKey => Id;
        // [JsonIgnore]
        public string UserKey => UserId;
        // internal 
        public CommunicationUser CommunicationUser { get; private set; }
        // internal 
        public CommunicationUserToken CommunicationUserToken { get; private set; }
        // internal 
        public ChatThreadMember ChatThreadMember { get; private set; }
        public User() { }
        public User(string userId, CommunicationUser communicationUser)
        {
            UserId = userId;
            CommunicationUser = communicationUser;
            ChatThreadMember = new ChatThreadMember(CommunicationUser) {
                DisplayName = UserId,
                ShareHistoryTime = new System.DateTimeOffset(),
                User = communicationUser
            };
        }
        public User(string userId, CommunicationUserToken token)
            : this(userId, token.User)
        {
            CommunicationUserToken = token;
        }

        public override bool Equals(object obj)
             => obj is User e && GetHashCode() == e.GetHashCode();

        public override int GetHashCode()
            => InternalKey.GetHashCode() ^ UserKey.GetHashCode();
    }
}
