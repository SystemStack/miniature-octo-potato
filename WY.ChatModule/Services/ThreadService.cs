using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Azure.Communication.Chat;
using Azure.Communication.Identity;
using ChatModule.Models;

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
            ChatThreadClient chatThreadClient = await Client.CreateChatThreadAsync(topic, members);
            ChatService chatService = new ChatService(chatThreadClient);
            Thread thread = new Thread(topic, chatThreadClient.Id, members);
            Store.Add(thread.Id, thread);
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
        public Azure.Response<ChatThread> GetChatThread(string idOrTopic) => GetChatThreadAsync(idOrTopic).Result;
        public async Task<Azure.Response<ChatThread>> GetChatThreadAsync(string idOrTopic)
        {
            if (Store.Exists(idOrTopic))
            {
                return await Client.GetChatThreadAsync(idOrTopic);
            } else if(Store.GetByUserKey(idOrTopic, out string id))
            {
                return await Client.GetChatThreadAsync(id);
            }
            throw new Exception(string.Format("Thread with id or topic: {0} does not exist", idOrTopic));
        }
        #endregion Get Chat Thread

        #region Get Chat Thread Client
        public ChatService GetCommunicationThreadClient(string idOrTopic)
        {
            ChatThreadClient chatThreadClient = Client.GetChatThreadClient(idOrTopic);
            return new ChatService(chatThreadClient);
        }
        #endregion Get Chat Thread Client
        public Azure.AsyncPageable<ChatThreadInfo> GetUserThreadsInfo(DateTimeOffset startTime = default) => GetUserThreadsInfoAsync(startTime);
        public Azure.AsyncPageable<ChatThreadInfo> GetUserThreadsInfoAsync(DateTimeOffset startTime = default)
        {
            return Client.GetChatThreadsInfoAsync(startTime);
        }

        private static ChatThreadMember IdentityToChatThreadMember(string idToken)
            => new ChatThreadMember(new Azure.Communication.CommunicationUser(idToken));
    }
}