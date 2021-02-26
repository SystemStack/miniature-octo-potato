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
            Utils.IsNotNull(user, nameof(user));
            CommunicationUserCredential communicationUserCredential = new CommunicationUserCredential(user.Token);
            Client = new ChatClient(AccessTokens.UriEndpoint, communicationUserCredential);
        }

        #region Create Chat Thread
        public Thread CreateChatThread(string topic, string identityToken)
            => CreateChatThread(topic, new List<string>() { identityToken });
        public Thread CreateChatThread(string topic, List<string> identities)
            => CreateChatThread(topic, identities.ConvertAll(new Converter<string, ChatThreadMember>(IdentityToChatThreadMember)));
        public Thread CreateChatThread(string topic, IEnumerable<ChatThreadMember> identities)
            => CreateChatThreadAsync(topic, identities).Result;
        public async Task<Thread> CreateChatThreadAsync(string topic, string identityToken)
            => await CreateChatThreadAsync(topic, new List<string>() { identityToken });
        public async Task<Thread> CreateChatThreadAsync(string topic, List<string> identities)
            => await CreateChatThreadAsync(topic, identities.ConvertAll(new Converter<string, ChatThreadMember>(IdentityToChatThreadMember)));
        public async Task<Thread> CreateChatThreadAsync(string topic, IEnumerable<ChatThreadMember> members)
        {
            Utils.IsNotNull(topic, nameof(topic));
            Utils.IsNotNullOrEmpty(members, nameof(members));
            if (Store.Exists(topic))
            {
                throw new Exception("Thread with topic already exists");
            }
            var chatThreadClient = await Client.CreateChatThreadAsync(topic, members);
            Thread thread = new Thread(chatThreadClient.Id, topic);
            Store.Add(thread);
            return thread;
        }
        #endregion Create Chat Thread

        #region Delete Chat Thread
        public Azure.Response DeleteChatThread(string idOrTopic) => DeleteChatThreadAsync(idOrTopic).Result;
        public async Task<Azure.Response> DeleteChatThreadAsync(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            if (Store.Exists(idOrTopic))
            {
                return await Client.DeleteChatThreadAsync(idOrTopic);
            }
            else if (Store.GetByUserKey(idOrTopic, out var id))
            {
                return await Client.DeleteChatThreadAsync(id);
            }
            throw new Exception(string.Format("Thread with id or topic: {0} does not exist", idOrTopic));
        }

        #endregion Delete Chat Thread

        #region GetChatThread
        public Thread GetChatThread(string idOrTopic) => GetChatThreadAsync(idOrTopic).Result;
        public async Task<Thread> GetChatThreadAsync(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            if (Store.Exists(idOrTopic))
            {
                return Store.Get(idOrTopic);
            }
            var result = await Client.GetChatThreadAsync(idOrTopic);
            if (Utils.IsFailure(result.GetRawResponse()))
            {
                throw new Exception(string.Format("Thread with id or topic: {0} does not exist", idOrTopic));
            }
            return new Thread(result.Value.Id, result.Value.Topic);
        }
        #endregion Get Chat Thread

        #region Get Chat Thread Client
        public ChatService GetCommunicationThreadClient(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            if (Store.Exists(idOrTopic))
            {
                return new ChatService(Client.GetChatThreadClient(idOrTopic));
            }
            else if (Store.GetByUserKey(idOrTopic, out var id))
            {
                return new ChatService(Client.GetChatThreadClient(id));
            }
            throw new Exception(string.Format("Could not find a thread with this topic or id {0}", idOrTopic));
        }
        #endregion Get Chat Thread Client
        public IEnumerable<ChatThreadInfo> GetUserThreadsInfo(DateTimeOffset startTime = default) => GetUserThreadsInfoAsync(startTime).Result;
        public async Task<IEnumerable<ChatThreadInfo>> GetUserThreadsInfoAsync(DateTimeOffset startTime = default)
        {
            return await Utils.AsyncToList(Client.GetChatThreadsInfoAsync(startTime));
        }

        // TODO Access Store to see if we can use the idtoken from there
        private ChatThreadMember IdentityToChatThreadMember(string idToken)
        {

            return new ChatThreadMember(new Azure.Communication.CommunicationUser(idToken));
        }
    }
}