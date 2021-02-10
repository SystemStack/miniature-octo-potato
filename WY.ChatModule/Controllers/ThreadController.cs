﻿using System;
using System.Threading.Tasks;
using ChatModule.Models;
using ChatModule.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ChatModule.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ThreadController : BaseController<ThreadService>
    {
        public ThreadController(IServiceProvider serviceProvider)
            : base(serviceProvider) {}
        
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<ChatService>> Create(string topic, [FromBody]List<string> members)
        {
            ChatService result = await Service.CreateChatThreadAsync(topic, members);

            return result;
        }
    }
}