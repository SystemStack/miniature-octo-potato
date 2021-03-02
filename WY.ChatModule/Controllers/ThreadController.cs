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
    [Route("api/[controller]s")]
    public sealed class ThreadController : BaseController<ThreadService>
    {
        public ThreadController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        #region Threads
        [HttpPost]
        public async Task<Thread> Create([FromBody] ThreadCreationModel thread)
        {
            Utils.IsNotNull(thread.Topic, nameof(thread.Topic));
            Utils.IsNotNullOrEmpty(thread.Members, nameof(thread.Members));
            return await Service.CreateChatThreadAsync(thread);
        }

        [HttpGet("{topic}")]
        public async Task<Thread> Get(string topic)
        {
            Utils.IsNotNull(topic, nameof(topic));
            return await Service.GetChatThreadAsync(topic);
        }

        [HttpDelete("{topic}")]
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

        #region Thread Members
        [HttpPost("{topic}/Members")]
        public async Task<bool> AddMembers(string topic, [FromBody] ThreadCreationModel model)
        {
            Utils.IsNotNull(topic ?? model.Topic, nameof(model.Topic));
            Utils.IsNotNullOrEmpty(model.Members, nameof(model.Members));
            var chatService = Service.GetCommunicationThreadClient(topic ?? model.Topic);
            var result = await chatService.AddMembersAsync(model.Members);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to add members to thread");
            }
            return true;
        }

        [HttpGet("{topic}/Members")]
        public async Task<IEnumerable<ChatThreadMember>> GetMembers(string topic)
        {
            Utils.IsNotNull(topic, nameof(topic));
            var chatService = Service.GetCommunicationThreadClient(topic);
            var result = await chatService.GetMembersAsync();
            if (!Utils.IsNotNullOrEmpty(result, nameof(result)))
            {
                throw new Exception("No members found");
            }
            return result;
        }

        [HttpDelete("{topic}/Members")]
        public async Task<int> RemoveMembers(string topic, [FromBody] ThreadCreationModel model)
        {
            Utils.IsNotNull(topic ?? model.Topic, nameof(model.Topic));
            Utils.IsNotNull(model.Members, nameof(model.Members));
            var chatService = Service.GetCommunicationThreadClient(topic ?? model.Topic);
            return await chatService.RemoveMembersAsync(model.Members);
        }
        #endregion Thread Memberships

        #region Thread Messages
        [HttpPost("{topic}/Messages")]
        public async Task<SendChatMessageResult> SendMessage(string topic, [FromBody] MessageCreationModel model)
        {
            Utils.IsNotNull(topic ?? model.Topic, nameof(model.Topic));
            Utils.IsNotNull(model.Content, nameof(model.Content));
            Utils.IsNotNull(model.CreatedBy, nameof(model.CreatedBy));
            var chatService = Service.GetCommunicationThreadClient(topic ?? model.Topic);
            return await chatService.SendMessageAsync(model.Content, model.CreatedBy, model.Priority);
        }

        [HttpGet("{topic}/Messages")]
        public async Task<IEnumerable<ChatMessage>> GetMessages(string topic)
        {
            Utils.IsNotNull(topic, nameof(topic));
            var chatService = Service.GetCommunicationThreadClient(topic);
            return await chatService.GetMessagesAsync();
        }

        [HttpPut("{topic}/Messages")]
        public async Task<bool> UpdateMessage(string topic, [FromBody] MessageCreationModel model)
        {
            Utils.IsNotNull(topic ?? model.Topic, nameof(topic));
            Utils.IsNotNull(model.Id, nameof(model.Id));
            Utils.IsNotNull(model.Content, nameof(model.Content));
            var chatService = Service.GetCommunicationThreadClient(topic ?? model.Topic);
            var result = await chatService.UpdateMessageAsync(model.Id, model.Content);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not update message");
            }
            return true;
        }

        [HttpGet("{topic}/{messageId}")]
        public async Task<ChatMessage> GetMessage(string topic, string messageId)
        {
            Utils.IsNotNull(topic, nameof(topic));
            Utils.IsNotNull(messageId, nameof(messageId));
            var chatService = Service.GetCommunicationThreadClient(topic);
            var result = await chatService.GetMessageAsync(messageId);
            if (Utils.IsFailure(result.GetRawResponse()))
            {
                throw new Exception("Could not find message with that Id");
            }
            return result.Value;
        }

        [HttpDelete("{topic}/{messageId}")]
        public async Task<bool> DeleteMessage(string topic, string messageId)
        {
            Utils.IsNotNull(topic, nameof(topic));
            Utils.IsNotNull(messageId, nameof(messageId));
            var chatService = Service.GetCommunicationThreadClient(topic);
            var result = await chatService.DeleteMessageAsync(messageId);

            if (Utils.IsFailure(result))
            {
                throw new Exception("Failed to delete message from thread");
            }
            return true;
        }
        #endregion Thread Messages

        #region Thread Message Features
        [HttpGet("{topic}/ReadReceipts")]
        public async Task<IEnumerable<ReadReceipt>> GetReadReceipts(string topic)
        {
            Utils.IsNotNull(topic, nameof(topic));
            var chatService = Service.GetCommunicationThreadClient(topic);
            return await chatService.GetReadReceiptsAsync();
        }

        [HttpPost("{topic}/{messageId}/ReadReceipts")]
        public async Task<bool> SendReadReceipt(string topic, string messageId)
        {
            throw new NotImplementedException(/*TODO: Read Receipts*/);
            Utils.IsNotNull(topic, nameof(topic));
            Utils.IsNotNull(messageId, nameof(messageId));
            var chatService = Service.GetCommunicationThreadClient(topic);
            var result = await chatService.SendReadReceiptAsync(messageId);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Could not send read receipt");
            }
            return true;
        }

        [HttpPost("{topic}/Typing")]
        public async Task<bool> SendTypingNotification(string topic)
        {
            throw new NotImplementedException(/*TODO: Read Receipts*/);
            Utils.IsNotNull(topic, nameof(topic));
            var chatService = Service.GetCommunicationThreadClient(topic);
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
