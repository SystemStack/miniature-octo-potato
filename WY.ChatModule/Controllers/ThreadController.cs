using Azure.Communication.Chat;
using ChatModule.Models;
using ChatModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatModule.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public sealed class ThreadController : BaseController<ThreadService>
    {
        public ThreadController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        #region Threads
        [HttpPost]
        public async Task<Thread> Create([FromBody] ThreadCreationModel thread)
        {
            Utils.IsNotNull(thread, nameof(thread));
            Utils.IsNotNullOrEmpty(thread.Members, nameof(thread.Members));
            return await Service.CreateChatThreadAsync(thread);
        }

        [HttpGet("{idOrTopic}")]
        public async Task<Thread> Get(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            return await Service.GetChatThreadAsync(idOrTopic);
        }

        [HttpDelete("{idOrTopic}")]
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
        [HttpPost("{idOrTopic}")]
        public async Task<bool> AddMembers(string idOrTopic, [FromBody] ThreadCreationModel model)
        {
            Utils.IsNotNull(idOrTopic ?? model.Topic, nameof(model.Topic));
            Utils.IsNotNullOrEmpty(model.Members, nameof(model.Members));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic ?? model.Topic);
            var result = await chatService.AddMembersAsync(model.Members);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to add members to thread");
            }
            return true;
        }

        [HttpGet("{idOrTopic}")]
        public async Task<IEnumerable<ChatThreadMember>> GetMembers(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.GetMembersAsync();
            return result;
        }

        [HttpPost("{idOrTopic}")]
        public async Task<int> RemoveMembers(string idOrTopic, [FromBody] ThreadCreationModel model)
        {
            Utils.IsNotNull(idOrTopic ?? model.Topic, nameof(model.Topic));
            Utils.IsNotNull(model.Members, nameof(model.Members));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic ?? model.Topic);
            return await chatService.RemoveMembersAsync(model.Members);
        }
        #endregion Thread Memberships

        #region Thread Messages
        [HttpPost("{idOrTopic}")]
        public async Task<SendChatMessageResult> SendMessage(string idOrTopic, [FromBody] MessageCreationModel model)
        {
            Utils.IsNotNull(idOrTopic ?? model.Topic, nameof(model.Topic));
            Utils.IsNotNull(model.Content, nameof(model.Content));
            Utils.IsNotNull(model.CreatedBy, nameof(model.CreatedBy));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic ?? model.Topic);
            return await chatService.SendMessageAsync(model.Content, model.CreatedBy, model.Priority);
        }

        [HttpGet("{idOrTopic}")]
        public async Task<IEnumerable<ChatMessage>> GetMessages(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            return await chatService.GetMessagesAsync();
        }

        [HttpPost("{idOrTopic}")]
        public async Task<bool> UpdateMessage(string idOrTopic, [FromBody] MessageCreationModel model)
        {
            Utils.IsNotNull(idOrTopic ?? model.Topic, nameof(idOrTopic));
            Utils.IsNotNull(model.Id, nameof(model.Id));
            Utils.IsNotNull(model.Content, nameof(model.Content));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic ?? model.Topic);
            var result = await chatService.UpdateMessageAsync(model.Id, model.Content);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not update message");
            }
            return true;
        }

        [HttpGet("{idOrTopic}/{messageId}")]
        public async Task<ChatMessage> GetMessage(string idOrTopic, string messageId)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            Utils.IsNotNull(messageId, nameof(messageId));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.GetMessageAsync(messageId);
            if (Utils.IsFailure(result.GetRawResponse()))
            {
                throw new Exception("Could not find message with that Id");
            }
            return result.Value;
        }

        [HttpDelete("{idOrTopic}/{messageId}")]
        public async Task<bool> DeleteMessage(string idOrTopic, string messageId)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            Utils.IsNotNull(messageId, nameof(messageId));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.DeleteMessageAsync(messageId);

            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to add members to thread");
            }
            return true;
        }
        #endregion Thread Messages

        #region Thread Message Features
        [HttpGet("{idOrTopic}")]
        public async Task<IEnumerable<ReadReceipt>> GetReadReceipts(string idOrTopic)
        {
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            return await chatService.GetReadReceiptsAsync();
        }

        [HttpPost("{idOrTopic}/{messageId}")]
        public async Task<bool> SendReadReceipt(string idOrTopic, string messageId)
        {
            throw new NotImplementedException(/*TODO*/);
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            Utils.IsNotNull(messageId, nameof(messageId));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.SendReadReceiptAsync(messageId);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not send read receipt");
            }
            return true;
        }

        [HttpPost("{idOrTopic}")]
        public async Task<bool> SendTypingNotification(string idOrTopic)
        {
            throw new NotImplementedException(/*TODO*/);
            Utils.IsNotNull(idOrTopic, nameof(idOrTopic));
            var chatService = Service.GetCommunicationThreadClient(idOrTopic);
            var result = await chatService.SendTypingNotificationAsync();
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not send typing notification");
            }
            return true;
        }
        #endregion Thread Message Features
    }
}
