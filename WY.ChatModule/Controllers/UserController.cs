using ChatModule.Models;
using ChatModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ChatModule.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]s")]
    public sealed class UserController : BaseController<UserService>
    {
        public UserController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        #region User CRUD
        [HttpGet("{userId}")]
        public ActionResult<User> Get(string userId) => Service.GetUser(userId);

        [HttpPost("{userId}")]
        public async Task<ActionResult<User>> Create(string userId)
        {
            userId ??= Guid.NewGuid().ToString();
            return await Service.CreateUserAsync(userId);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<User>> Delete(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.DeleteUserAsync(userId);
        }
        #endregion User CRUD

        #region User Actions
        [HttpGet("{userId}/Refresh")]
        public async Task<ActionResult<User>> RefreshToken(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.RefreshTokenAsync(userId);
        }

        [HttpGet("{userId}/Revoke")]
        public async Task<ActionResult<User>> RevokeToken(string userId)
        {
            Utils.IsNotNull(userId, nameof(userId));
            return await Service.RevokeAccessTokenAsync(userId);
        }
        #endregion User Actions

    }
}
