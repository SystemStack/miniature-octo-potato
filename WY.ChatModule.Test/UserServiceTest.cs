using System;
using ChatModule.Models;
using ChatModule.Services;
using ChatModule.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace ChatModule.Test
{
    // Integration tests
    public class UserServiceTest
    {
        
        private readonly UserService userService;
        public UserServiceTest(IServiceProvider serviceProvider)
        {
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new Store<User>())
                            .AddSingleton(new Store<Thread>());
                });
            userService = (UserService)serviceProvider.GetService(typeof(UserService));
        }

        [Fact]
        public void TestCreateUser()
        {
            // Random username
            var userName = Guid.NewGuid().ToString();
            var user = userService.CreateUser(userName);
            Assert.Equal(userName, user.UserId);
            Assert.NotNull(user.Id);

            // Needs a valid non expired token
            Assert.NotNull(user.CommunicationUserToken.Token);
            Assert.True(user.CommunicationUserToken.ExpiresOn > DateTime.Now, "Token expired");

            // Teardown
            Assert.True(Utils.IsSuccess(userService.DeleteUser(userName)));
        }
    }
}
