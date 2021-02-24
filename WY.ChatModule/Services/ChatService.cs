using Azure.Communication;
using Azure.Communication.Chat;
using ChatModule.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ChatModule.Test")]
namespace ChatModule.Services
{
    public sealed class ChatService
    {
        private ChatThreadClient Client { get; }
        public ChatService(ChatThreadClient chatThreadClient)
        {
            Utils.IsNotNull(chatThreadClient, nameof(chatThreadClient));
            Client = chatThreadClient;
        }

        #region Members
        public Azure.Response AddMembers(List<User> chatMembers)
            => AddMembersAsync(chatMembers.ConvertAll(new Converter<User, ChatThreadMember>(UserToChatThreadMember))).Result;
        public async Task<Azure.Response> AddMembersAsync(List<User> chatMembers)
            => await AddMembersAsync(chatMembers.ConvertAll(new Converter<User, ChatThreadMember>(UserToChatThreadMember)));
        public Azure.Response AddMembers(IEnumerable<ChatThreadMember> chatMembers) => AddMembersAsync(chatMembers).Result;
        public async Task<Azure.Response> AddMembersAsync(IEnumerable<ChatThreadMember> chatMembers)
        {
            Utils.IsNotNullOrEmpty(chatMembers, nameof(chatMembers));
            return await Client.AddMembersAsync(chatMembers);
        }
        
        public IEnumerable<ChatThreadMember> GetMembers() => GetMembersAsync().Result;
        public async Task<IEnumerable<ChatThreadMember>> GetMembersAsync()
        {
            return await Utils.AsyncToList(Client.GetMembersAsync());
        }

        public Azure.Response RemoveMember(User user)
           => RemoveMemberAsync(user.CommunicationUser).Result;
        public async Task<Azure.Response> RemoveMemberAsync(User user)
            => await RemoveMemberAsync(user.CommunicationUser);
        public Azure.Response RemoveMember(CommunicationUser user)
            => RemoveMemberAsync(user).Result;
        public async Task<Azure.Response> RemoveMemberAsync(CommunicationUser user)
        {
            Utils.IsNotNull(user, nameof(user));
            return await Client.RemoveMemberAsync(user);
        }
        #endregion Members

        #region Messages
        public SendChatMessageResult SendMessage(string content, string? displayName = null) => SendMessageAsync(content, displayName).Result;
        public async Task<SendChatMessageResult> SendMessageAsync(string content, string? displayname = null)
        {
            Utils.IsNotNull(content, nameof(content));
            return await Client.SendMessageAsync(content, ChatMessagePriority.Normal, displayname);
        }

        public Azure.Response SendReadReceipt(string messageId) => SendReadReceiptAsync(messageId).Result;
        public async Task<Azure.Response> SendReadReceiptAsync(string messageId)
        {
            Utils.IsNotNull(messageId, nameof(messageId));
            return await Client.SendReadReceiptAsync(messageId);
        }

        public Azure.Response SendTypingNotification() => SendTypingNotificationAsync().Result;
        public async Task<Azure.Response> SendTypingNotificationAsync()
        {
            return await Client.SendTypingNotificationAsync();
        }

        public IEnumerable<ChatMessage> GetMessages() => GetMessagesAsync().Result;
        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync()
        {
            return await Utils.AsyncToList(Client.GetMessagesAsync());
        }
        public IEnumerable<ReadReceipt> GetReadReceipts() => GetReadReceiptsAsync().Result;
        public async Task<IEnumerable<ReadReceipt>> GetReadReceiptsAsync()
        {
            return await Utils.AsyncToList(Client.GetReadReceiptsAsync());
        }

        public Azure.Response UpdateMessage(string messageId, string content) => UpdateMessageAsync(messageId, content).Result;
        public async Task<Azure.Response> UpdateMessageAsync(string messageId, string content)
        {
            Utils.IsNotNull(messageId, nameof(messageId));
            Utils.IsNotNull(content, nameof(content));
            return await Client.UpdateMessageAsync(messageId, content);
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
        #endregion Messages


        public Azure.Response UpdateThread(string topic) => UpdateThreadAsync(topic).Result;
        public async Task<Azure.Response> UpdateThreadAsync(string topic)
        {
            Utils.IsNotNull(topic, nameof(topic));
            return await Client.UpdateThreadAsync(topic);
        }

        private static ChatThreadMember UserToChatThreadMember(User user)
            => new ChatThreadMember(user.CommunicationUser);
    }
}