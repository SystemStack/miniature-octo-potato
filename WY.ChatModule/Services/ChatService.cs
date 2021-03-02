using Azure.Communication;
using Azure.Communication.Chat;
using ChatModule.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatModule.Services
{
    public sealed class ChatService : BaseService<User, ChatThreadClient>
    {
        public ChatService(IServiceProvider serviceProvider, ChatThreadClient chatThreadClient)
            : base(serviceProvider)
        {
            Utils.IsNotNull(chatThreadClient, nameof(chatThreadClient));
            Client = chatThreadClient;
        }

        #region Add Thread Members
        public Azure.Response AddMembers(List<string> chatMembers) => AddMembersAsync(chatMembers).Result;
        public async Task<Azure.Response> AddMembersAsync(List<string> chatMembers)
            => await AddMembersAsync(chatMembers.ConvertAll(new Converter<string, User>(StringToUser)));
        public Azure.Response AddMembers(List<User> chatMembers) => AddMembersAsync(chatMembers).Result;
        public async Task<Azure.Response> AddMembersAsync(List<User> chatMembers)
            => await AddMembersAsync(chatMembers.ConvertAll(new Converter<User, ChatThreadMember>(UserToChatThreadMember)));
        public Azure.Response AddMembers(IEnumerable<ChatThreadMember> chatMembers) => AddMembersAsync(chatMembers).Result;
        public async Task<Azure.Response> AddMembersAsync(IEnumerable<ChatThreadMember> chatMembers)
        {
            return await Client.AddMembersAsync(chatMembers);
        }
        #endregion Add Thread Members

        #region Get Thread Members
        public IEnumerable<ChatThreadMember> GetMembers() => GetMembersAsync().Result;
        public async Task<IEnumerable<ChatThreadMember>> GetMembersAsync()
        {
            return await Utils.AsyncToList(Client.GetMembersAsync());
        }
        #endregion Get Thread Members

        #region Remove Thread Members
        public async Task<int> RemoveMembersAsync(List<string> members)
        {
            var tasks = new List<Task>();
            var removedMembers = 0;
            foreach (var member in members)
            {
                tasks.Add(Task.Run(async () => {
                    if (Utils.IsSuccess(await RemoveMemberAsync(member)))
                    {
                        Interlocked.Increment(ref removedMembers);
                    }
                }));
            }
            await Task.WhenAll(tasks);
            return removedMembers;
        }

        public Azure.Response RemoveMember(string id)
            => RemoveMemberAsync(Store.Get(id).CommunicationUser).Result;
        public async Task<Azure.Response> RemoveMemberAsync(string id)
            => await RemoveMemberAsync(Store.Get(id).CommunicationUser);
        public Azure.Response RemoveMember(User user)
           => RemoveMemberAsync(user.CommunicationUser).Result;
        public async Task<Azure.Response> RemoveMemberAsync(User user)
            => await RemoveMemberAsync(user.CommunicationUser);
        public Azure.Response RemoveMember(CommunicationUser user)
            => RemoveMemberAsync(user).Result;
        public async Task<Azure.Response> RemoveMemberAsync(CommunicationUser user)
        {
            return await Client.RemoveMemberAsync(user);
        }
        #endregion Remove Thread Members

        #region Messages
        public SendChatMessageResult SendMessage(string content, string displayName) => SendMessageAsync(content, displayName).Result;
        public async Task<SendChatMessageResult> SendMessageAsync(string content, string displayname, ChatMessagePriority priorty = default)
        {
            return await Client.SendMessageAsync(content, priorty, displayname);
        }

        public IEnumerable<ChatMessage> GetMessages() => GetMessagesAsync().Result;
        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync()
        {
            return await Utils.AsyncToList(Client.GetMessagesAsync());
        }

        public Azure.Response UpdateMessage(string messageId, string content) => UpdateMessageAsync(messageId, content).Result;
        public async Task<Azure.Response> UpdateMessageAsync(string messageId, string content)
        {
            Utils.IsNotNull(messageId, nameof(messageId));
            Utils.IsNotNull(content, nameof(content));
            return await Client.UpdateMessageAsync(messageId, content);
        }
        public Azure.Response UpdateThread(string topic) => UpdateThreadAsync(topic).Result;
        public async Task<Azure.Response> UpdateThreadAsync(string topic)
        {
            Utils.IsNotNull(topic, nameof(topic));
            return await Client.UpdateThreadAsync(topic);
        }

        public Azure.Response DeleteMessage(string messageId) => DeleteMessageAsync(messageId).Result;
        public async Task<Azure.Response> DeleteMessageAsync(string messageId)
        {
            Utils.IsNotNull(messageId, nameof(messageId));
            return await Client.DeleteMessageAsync(messageId);
        }
        public Azure.Response<ChatMessage> GetMessage(string id) => GetMessageAsync(id).Result;
        public async Task<Azure.Response<ChatMessage>> GetMessageAsync(string id)
        {
            Utils.IsNotNull(id, nameof(id));
            return await Client.GetMessageAsync(id);
        }
        #endregion

        #region Read Receipts & IsTyping
        public Azure.Response SendTypingNotification() => SendTypingNotificationAsync().Result;
        public async Task<Azure.Response> SendTypingNotificationAsync()
        {
            return await Client.SendTypingNotificationAsync();
        }

        public Azure.Response SendReadReceipt(string messageId) => SendReadReceiptAsync(messageId).Result;
        public async Task<Azure.Response> SendReadReceiptAsync(string messageId)
        {
            return await Client.SendReadReceiptAsync(messageId);
        }
        public IEnumerable<ReadReceipt> GetReadReceipts() => GetReadReceiptsAsync().Result;
        public async Task<IEnumerable<ReadReceipt>> GetReadReceiptsAsync()
        {
            return await Utils.AsyncToList(Client.GetReadReceiptsAsync());
        }
        #endregion Read Receipts


        private static ChatThreadMember UserToChatThreadMember(User user)
            => user.ChatThreadMember;

        private User StringToUser(string userId)
            => Store.Get(userId);
    }
}