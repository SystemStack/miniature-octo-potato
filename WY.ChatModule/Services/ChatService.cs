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
            Client = chatThreadClient;
        }
        public Azure.Response AddMembers(List<User> chatMembers)
            => AddMembersAsync(chatMembers.ConvertAll(new Converter<User, ChatThreadMember>(UserToChatThreadMember))).Result;
        public async Task<Azure.Response> AddMembersAsync(List<User> chatMembers)
            => await AddMembersAsync(chatMembers.ConvertAll(new Converter<User, ChatThreadMember>(UserToChatThreadMember)));
        public Azure.Response AddMembers(IEnumerable<ChatThreadMember> chatMembers) => AddMembersAsync(chatMembers).Result;
        public async Task<Azure.Response> AddMembersAsync(IEnumerable<ChatThreadMember> chatMembers)
        {
            return await Client.AddMembersAsync(chatMembers);
        }

        public bool DeleteMessage(string param1) => DeleteMessageAsync(param1).Result;
        public async Task<bool> DeleteMessageAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.DeleteMessageAsync();
            //return x;
        }
        public IEnumerable<User> GetMembers(string param1) => GetMembersAsync(param1).Result;
        public async Task<IEnumerable<User>> GetMembersAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.GetMembersAsync();
            //return x;
        }
        public object GetMessage(string param1) => GetMessageAsync(param1).Result;
        public async Task<object> GetMessageAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.GetMessageAsync();
            //return x;
        }
        public IEnumerable<object> GetMessages(string param1) => GetMessagesAsync(param1).Result;
        public async Task<IEnumerable<object>> GetMessagesAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.GetMessagesAsync();
            //return x;
        }
        public IEnumerable<object> GetReadReceipts(string param1) => GetReadReceiptsAsync(param1).Result;
        public async Task<IEnumerable<object>> GetReadReceiptsAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.GetReadReceiptsAsync();
            //return x;
        }
        public bool RemoveMember(string param1) => RemoveMemberAsync(param1).Result;
        public async Task<bool> RemoveMemberAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.RemoveMemberAsync();
            //return x;
        }
        public bool SendMessage(string param1) => SendMessageAsync(param1).Result;
        public async Task<bool> SendMessageAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.SendMessageAsync();
            //return x;
        }
        public object SendReadReceipt(string param1) => SendReadReceiptAsync(param1).Result;
        public async Task<object> SendReadReceiptAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.SendReadReceiptAsync();
            //return x;
        }
        public object SendTypingNotification(string param1) => SendTypingNotificationAsync(param1).Result;
        public async Task<object> SendTypingNotificationAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.SendTypingNotificationAsync();
            //return x;
        }
        public bool UpdateMessage(string param1) => UpdateMessageAsync(param1).Result;
        public async Task<bool> UpdateMessageAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.UpdateMessageAsync();
            //return x;
        }
        public bool UpdateThread(string param1) => UpdateThreadAsync(param1).Result;
        public async Task<bool> UpdateThreadAsync(string param1)
        {
            throw new NotImplementedException(/*TODO: Implement*/);
            //var x = await Client.UpdateThreadAsync();
            //return x;
        }

        private static ChatThreadMember UserToChatThreadMember(User user)
            => new ChatThreadMember(user.CommunicationUser);

    }
}