using ChatModule.Models;
using ChatModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ChatModule.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public sealed class UserController : BaseController<UserService>
    {
        public UserController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        [HttpGet("{userId}")]
        public ActionResult<User> Get(string userId) => Service.GetUser(userId);

        [HttpPost("{userId}")]
        public async Task<ActionResult<User>> Create(string userId)
        {
            userId ??= Guid.NewGuid().ToString();
            return await Service.CreateUserAsync(userId);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<User>> RefreshToken(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.RefreshTokenAsync(userId);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<User>> RevokeToken(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.RevokeAccessTokenAsync(userId);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<User>> Delete(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.DeleteUserAsync(userId);
        }
    }
}
