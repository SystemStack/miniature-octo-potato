﻿using ChatModule.Models;
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

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<ChatService>> Create(string topic, [FromBody] List<string> members)
        {
            Utils.IsNotNull(topic, nameof(topic));
            return await Service.CreateChatThreadAsync(topic, members); ;
        }

        [HttpDelete]
        [ApiVersion("1.0")]
        public async Task<Thread> Get(string topic)
        {
            Utils.IsNotNull(topic, nameof(topic));
            return await Service.GetChatThreadAsync(topic);
        }


        [HttpDelete]
        [ApiVersion("1.0")]
        public async Task<bool> Delete(string topic)
        {
            Utils.IsNotNull(topic, nameof(topic));
            var result = await Service.DeleteChatThreadAsync(topic);
            if (Utils.IsFailure(result))
            {
                throw new Exception("Thread not deleted");
            }
            return true;
        }

    }
}
