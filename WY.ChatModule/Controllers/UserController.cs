using ChatModule.Models;
using ChatModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ChatModule.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public sealed class UserController : BaseController<UserService>
    {
        public UserController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        [HttpPost("Create/{userId}")]
        [ApiVersion("1.0")]
        public async Task<ActionResult<User>> Create([FromRoute] string userId)
        {
            userId ??= Guid.NewGuid().ToString();
            return await Service.CreateUserAsync(userId);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<User>> RefreshToken(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.RefreshTokenAsync(userId);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<Azure.Response>> RevokeAccessToken(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.RevokeAccessTokenAsync(userId);
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<Azure.Response>> Delete(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.DeleteUserAsync(userId);
        }
    }
}
