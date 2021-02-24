using Azure.Communication.Chat;
using ChatModule.Models;
using ChatModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// TODO refactor controllers to make chatthreadclient/chatclient exposure more logical 
namespace ChatModule.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public sealed class ChatController : BaseController<ChatService>
    {
        public ChatController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> AddMembers(IEnumerable<User> chatMembers)
        {
            
            var result = await Service.AddMembersAsync(chatMembers as List<User>);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to add members to thread");
            }
            return result;
        }

        [HttpDelete]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> DeleteMessage(string messageId)
        {
            var result = await Service.DeleteMessageAsync(messageId);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to add members to thread");
            }
            return result;
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<User>> GetMembers(string param1)
        {
            throw new NotImplementedException(/*TODO: MAP chatthreadmembers to users*/);
            var result = await Service.GetMembersAsync();
            return result as IEnumerable<User>;
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ChatMessage> GetMessage(string messageId)
        {
            var result = await Service.GetMessageAsync(messageId);
            if (Utils.IsFailure(result.GetRawResponse()))
            {
                throw new Exception("Could not find message with that Id");
            }
            return result.Value;
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<ChatMessage>> GetMessages()
        {
            var result = await Service.GetMessagesAsync();
            return result;
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<ReadReceipt>> GetReadReceipts()
        {
            return await Service.GetReadReceiptsAsync();
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> RemoveMember(User user)
        {
            var result = await Service.RemoveMemberAsync(user);
            if (Utils.IsFailure(result))
            {
                throw new Exception("User was not removed");
            }
            return result;
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<SendChatMessageResult> SendMessage(string content, [FromBody] User? user)
        {
            return await Service.SendMessageAsync(content, user?.UserId);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> SendReadReceipt(string messageId)
        {
            var result = await Service.SendReadReceiptAsync(messageId);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not send read receipt");
            }
            return result;
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> SendTypingNotification()
        {
            var result = await Service.SendTypingNotificationAsync();
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not send typing notification");
            }
            return result;
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> UpdateMessage(string messageId, string content)
        {
            var result = await Service.UpdateMessageAsync(messageId, content);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not update message");
            }
            return result;
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> UpdateThread(string topic)
        {
            var response = await Service.UpdateThreadAsync(topic);
            if (Utils.IsFailure(response))
            {
                throw new Exception("Could not update thread");
            }
            return response;
        }
    }
}
