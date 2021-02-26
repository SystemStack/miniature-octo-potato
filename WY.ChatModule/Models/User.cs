using Azure.Communication;
using Azure.Communication.Administration.Models;
using Azure.Communication.Chat;
using System;

namespace ChatModule.Models
{
    public class User : IModel
    {
        public string UserId { get; set; }
        public string Id => CommunicationUser.Id;
        public string Token => CommunicationUserToken.Token;
        public DateTimeOffset? ShareHistoryTime => ChatThreadMember.ShareHistoryTime;
        public string InternalKey => Id;
        public string UserKey => UserId;
        
        public CommunicationUser CommunicationUser { get; private set; }
        private CommunicationUserToken CommunicationUserToken { get; set; }
        private ChatThreadMember ChatThreadMember { get; set; }
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
