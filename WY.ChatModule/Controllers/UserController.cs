using System;
using System.Threading.Tasks;
using ChatModule.Models;
using ChatModule.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatModule.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class UserController : BaseController<UserService>
    {
        public UserController(IServiceProvider serviceProvider) 
            : base(serviceProvider) {}

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<User>> Create(string userId)
        {
            userId ??= Guid.NewGuid().ToString();
            return await Service.CreateUserAsync(userId);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<User>> RefreshToken(string userId)
        {
            return await Service.RefreshTokenAsync(userId);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<Azure.Response>> RevokeAccessToken(string userId)
        {
            return await Service.RevokeAccessTokenAsync(userId);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<Azure.Response>> Delete(string userId)
        {
            return await Service.DeleteUserAsync(userId);
        }
    }
}
