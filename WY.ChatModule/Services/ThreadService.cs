using Azure.Communication.Chat;
using Azure.Communication.Identity;
using ChatModule.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ChatModule.Test")]
namespace ChatModule.Services
{
    public sealed class ThreadService : BaseService<Thread, ChatClient>
    {
        public ThreadService(IServiceProvider serviceProvider, User user)
            : base(serviceProvider)
        {
            CommunicationUserCredential communicationUserCredential = new CommunicationUserCredential(user.Token);
            Client = new ChatClient(AccessTokens.UriEndpoint, communicationUserCredential);
        }

        #region Create Chat Thread
        public ChatService CreateChatThread(string topic, string identityToken)
            => CreateChatThread(topic, new List<string>() { identityToken });
        public ChatService CreateChatThread(string topic, List<string> identities)
            => CreateChatThread(topic, identities.ConvertAll(new Converter<string, ChatThreadMember>(IdentityToChatThreadMember)));
        public ChatService CreateChatThread(string topic, IEnumerable<ChatThreadMember> identities)
            => CreateChatThreadAsync(topic, identities).Result;
        public async Task<ChatService> CreateChatThreadAsync(string topic, string identityToken)
            => await CreateChatThreadAsync(topic, new List<string>() { identityToken });
        public async Task<ChatService> CreateChatThreadAsync(string topic, List<string> identities)
            => await CreateChatThreadAsync(topic, identities.ConvertAll(new Converter<string, ChatThreadMember>(IdentityToChatThreadMember)));
        public async Task<ChatService> CreateChatThreadAsync(string topic, IEnumerable<ChatThreadMember> members)
        {
            var chatThreadClient = await Client.CreateChatThreadAsync(topic, members);
            var chatService = new ChatService(chatThreadClient);
            var clientThread = Client.GetChatThread(chatThreadClient.Id);
            Store.Add(topic, new Thread(clientThread));
            return chatService;
        }
        #endregion Create Chat Thread

        #region Delete Chat Thread
        public Azure.Response DeleteChatThread(string idOrTopic) => DeleteChatThreadAsync(idOrTopic).Result;
        public async Task<Azure.Response> DeleteChatThreadAsync(string idOrTopic)
        {
            return await Client.DeleteChatThreadAsync(idOrTopic);
        }
        #endregion Delete Chat Thread

        #region GetChatThread
        public Thread GetChatThread(string idOrTopic) => GetChatThreadAsync(idOrTopic).Result;
        public async Task<Thread> GetChatThreadAsync(string idOrTopic)
        {
            if (Store.Exists(idOrTopic))
            {
                return new Thread(await Client.GetChatThreadAsync(idOrTopic));
            }
            else if (Store.GetByUserKey(idOrTopic, out var id))
            {
                return new Thread(await Client.GetChatThreadAsync(id));
            }
            throw new Exception(string.Format("Thread with id or topic: {0} does not exist", idOrTopic));
        }
        #endregion Get Chat Thread

        #region Get Chat Thread Client
        public ChatService GetCommunicationThreadClient(string idOrTopic)
        {
            var chatThreadClient = Client.GetChatThreadClient(idOrTopic);
            return new ChatService(chatThreadClient);
        }
        #endregion Get Chat Thread Client
        public Azure.AsyncPageable<ChatThreadInfo> GetUserThreadsInfo(DateTimeOffset startTime = default) => GetUserThreadsInfoAsync(startTime);
        public Azure.AsyncPageable<ChatThreadInfo> GetUserThreadsInfoAsync(DateTimeOffset startTime = default)
        {
            return Client.GetChatThreadsInfoAsync(startTime);
        }

        // TODO Access Store to see if we can use the idtoken from there
        private static ChatThreadMember IdentityToChatThreadMember(string idToken)
            => new ChatThreadMember(new Azure.Communication.CommunicationUser(idToken));
    }
}