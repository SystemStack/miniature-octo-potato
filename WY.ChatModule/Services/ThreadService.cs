using Azure.Communication.Chat;
using Azure.Communication.Identity;
using ChatModule.Models;
using ChatModule.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ChatModule.Services
{
    public sealed class ThreadService : BaseService<Thread, ChatClient>
    {
        private Store<User> UserStore { get; }
        public ThreadService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            Utils.IsNotNull(serviceProvider, nameof(serviceProvider));
            CommunicationUserCredential adminThreadCredential = new CommunicationUserCredential(UserService.ChatBotUser.CommunicationUserToken.Token);
            Client = new ChatClient(AccessTokens.UriEndpoint, adminThreadCredential);
            UserStore = (Store<User>)serviceProvider.GetService(typeof(Store<User>));
        }

        #region Create Chat Thread
        public async Task<Thread> CreateChatThreadAsync(ThreadCreationModel thread)
            => await CreateChatThreadAsync(thread.Topic, thread.Members.ConvertAll(new Converter<string, User>(StringToUser)));
        public async Task<Thread> CreateChatThreadAsync(string topic, List<User> members)
        {
            if (Store.Exists(topic))
            {
                throw new Exception("Thread with topic already exists");
            }
            members.Add(UserService.ChatBotUser);
            var chatThreadMembers = members.ConvertAll(new Converter<User, ChatThreadMember>(UserToChatThreadMember));
            var chatThreadClient = await Client.CreateChatThreadAsync(topic, chatThreadMembers);
            Thread thread = new Thread(chatThreadClient.Id, topic, members);
            Store.Add(thread);
            return thread;
        }
        #endregion Create Chat Thread

        #region Delete Chat Thread
        public Azure.Response DeleteChatThread(string idOrTopic) => DeleteChatThreadAsync(idOrTopic).Result;
        public async Task<Azure.Response> DeleteChatThreadAsync(string idOrTopic)
        {
            if (Store.Exists(idOrTopic))
            {
                return await Client.DeleteChatThreadAsync(Store.Get(idOrTopic).Id);
            }
            throw new Exception(string.Format("Thread with id or topic: {0} does not exist", idOrTopic));
        }
        #endregion Delete Chat Thread

        #region GetChatThread
        public Thread GetChatThread(string idOrTopic) => GetChatThreadAsync(idOrTopic).Result;
        public async Task<Thread> GetChatThreadAsync(string idOrTopic)
        {
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
            if (Store.Exists(idOrTopic))
            {
                var key = Store.Get(idOrTopic).Id;
                return new ChatService(ServiceProvider, Client.GetChatThreadClient(key));
            }
            throw new Exception(string.Format("Could not find a thread with this topic or id {0}", idOrTopic));
        }
        public IEnumerable<ChatThreadInfo> GetUserThreadsInfo(DateTimeOffset startTime = default) => GetUserThreadsInfoAsync(startTime).Result;
        public async Task<IEnumerable<ChatThreadInfo>> GetUserThreadsInfoAsync(DateTimeOffset startTime = default)
        {
            return await Utils.AsyncToList(Client.GetChatThreadsInfoAsync(startTime));
        }
        #endregion Get Chat Thread Client

        #region Helpers
        private User StringToUser(string id)
            => UserStore.Get(id);
        private ChatThreadMember UserToChatThreadMember(User user)
            => user.ChatThreadMember;
        #endregion Helpers
    }
}