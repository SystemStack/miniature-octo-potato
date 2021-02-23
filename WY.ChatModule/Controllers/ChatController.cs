using ChatModule.Models;
using ChatModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatModule.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ChatController : BaseController<ChatService>
    {
        public ChatController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<bool> AddMembers(IEnumerable<User> chatMembers)
        {
            var result = await Service.AddMembersAsync(chatMembers as List<User>);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to add members to thread");
            }
            return true;
        }

        [HttpDelete]
        [ApiVersion("1.0")]
        public async Task<bool> DeleteMessage(string param1)
        {
            return await Service.DeleteMessageAsync(param1);
        }


        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<User>> GetMembers(string param1)
        {
            return await Service.GetMembersAsync(param1);
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<object> GetMessage(string param1)
        {
            return await Service.GetMessageAsync(param1);
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<object>> GetMessages(string param1)
        {
            return await Service.GetMessagesAsync(param1);
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<object>> GetReadReceipts(string param1)
        {
            return await Service.GetReadReceiptsAsync(param1);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<bool> RemoveMember(string param1)
        {
            return await Service.RemoveMemberAsync(param1);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<bool> SendMessage(string param1)
        {
            return await Service.SendMessageAsync(param1);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<object> SendReadReceipt(string param1)
        {
            return await Service.SendReadReceiptAsync(param1);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<object> SendTypingNotification(string param1)
        {
            return await Service.SendTypingNotificationAsync(param1);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<bool> UpdateMessage(string param1)
        {
            return await Service.UpdateMessageAsync(param1);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<bool> UpdateThread(string param1)
        {
            return await Service.UpdateThreadAsync(param1);
        }
    }
}
