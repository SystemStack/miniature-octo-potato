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
    public sealed class ThreadController : BaseController<ThreadService>
    {
        public ThreadController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        #region Threads
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Thread> Create(string topic, [FromBody] List<string> members)
        {
            Utils.IsNotNull(topic, nameof(topic));
            return await Service.CreateChatThreadAsync(topic, members); ;
        }

        [HttpDelete]
        [ApiVersion("1.0")]
        public async Task<Thread> Get(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            return await Service.GetChatThreadAsync(idOrTopic);
        }

        [HttpDelete]
        [ApiVersion("1.0")]
        public async Task<bool> Delete(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            var result = await Service.DeleteChatThreadAsync(idOrTopic);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Thread not deleted");
            }
            return true;
        }
        #endregion Threads

        #region Thread Memberships
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> AddMembers(string idOrTopic, IEnumerable<User> chatMembers)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.AddMembersAsync(chatMembers as List<User>);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to add members to thread");
            }
            return result;
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<User>> GetMembers(string idOrTopic)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.GetMembersAsync(); // TODO: Map ChatThreadMember to User

            return result as IEnumerable<User>;
        }


        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> RemoveMember(string idOrTopic, User user)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.RemoveMemberAsync(user);

            if (Utils.IsFailure(result))
            {
                throw new Exception("User was not removed");
            }
            return result;
        }

        #endregion Thread Memberships

        #region Thread Messages

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<SendChatMessageResult> SendMessage(string idOrTopic, string content, User user)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            return await chatService.SendMessageAsync(content, user?.UserId);
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ChatMessage> GetMessage(string idOrTopic, string messageId)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.GetMessageAsync(messageId);

            if (Utils.IsFailure(result.GetRawResponse()))
            {
                throw new Exception("Could not find message with that Id");
            }
            return result.Value;
        }

        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<ChatMessage>> GetMessages(string idOrTopic)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.GetMessagesAsync();

            return result;
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> UpdateMessage(string idOrTopic, string messageId, string content)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.UpdateMessageAsync(messageId, content);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not update message");
            }
            return result;
        }

        [HttpDelete]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> DeleteMessage(string idOrTopic, string messageId)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.DeleteMessageAsync(messageId);

            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to add members to thread");
            }
            return result;
        }
        #endregion Thread Messages



        #region Thread Message Features
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IEnumerable<ReadReceipt>> GetReadReceipts(string idOrTopic)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            return await chatService.GetReadReceiptsAsync();
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> SendReadReceipt(string idOrTopic, string messageId)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.SendReadReceiptAsync(messageId);

            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not send read receipt");
            }
            return result;
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<Azure.Response> SendTypingNotification(string idOrTopic)
        {
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.SendTypingNotificationAsync();

            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not send typing notification");
            }
            return result;
        }
        #endregion Thread Message Features
    }
}
