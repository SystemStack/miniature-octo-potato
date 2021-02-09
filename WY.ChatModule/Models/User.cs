using Azure.Communication;
using Azure.Communication.Administration.Models;
using System;

namespace ChatModule.Models
{
    public class User : IModel
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public string Token { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public CommunicationUserToken CommunicationUserToken { get; set; }
        public string InternalKey => Id;
        public string UserKey => UserId;
        public CommunicationUser CommunicationUser
            => CommunicationUserToken.User;

        public User(string userId)
        {
            UserId = userId;
        }
        public User(string userId, CommunicationUser communicationUser)
            : this(userId)
        {
            Id = communicationUser.Id;
        }
        public User(string userId, CommunicationUserToken token)
            : this(userId, token.User)
        {
            Token = token.Token;
            ExpiresOn = token.ExpiresOn;
            CommunicationUserToken = token;
        }
    }
}
