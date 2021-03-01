using Azure.Communication.Administration;
using ChatModule.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatModule.Services
{
    public sealed class UserService : BaseService<User, CommunicationIdentityClient>
    {
        private readonly List<CommunicationTokenScope> Scope =
            new List<CommunicationTokenScope>() { CommunicationTokenScope.Chat };
        private static readonly string ChatBotName = "Weyer Bot - 🤖"; // TODO Move to ChatBot namespace
        public static User ChatBotUser { get; set; }        
        public UserService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            try
            {
                Client = new CommunicationIdentityClient(AccessTokens.ConnectionString);
            }
            catch
            {
                Client = new CommunicationIdentityClient(AccessTokens.SecondaryConnectionString);
            }
            CreateChatBotUser();
        }

        public UserService(IServiceProvider serviceProvider, IEnumerable<CommunicationTokenScope> scopes)
            : this(serviceProvider)
        {
            Scope.AddRange(scopes);
        }

        public User GetUser(string userId) => Store.Get(userId);
        
        
        private void CreateChatBotUser()
        {
            if (Store.Exists(ChatBotName))
            {
                ChatBotUser = Store.Get(ChatBotName);
            } else
            {
                var createResponse = Client.IssueToken(Client.CreateUser(), Scope);
                if (Utils.IsFailure(createResponse))
                {
                    throw new Exception("Failed to create super admin account");
                }
                ChatBotUser = new User(ChatBotName, createResponse.Value);
                Store.Add(ChatBotUser);
            }
        }
        public User CreateUser(string userId) => CreateUserAsync(userId).Result;
        public async Task<User> CreateUserAsync(string userId)
        {
            if (Store.Exists(userId))
            {
                throw new Exception("User ID Already exists");
            }
            var createResponse = await Client.IssueTokenAsync(await Client.CreateUserAsync(), Scope);
            if (Utils.IsFailure(createResponse))
            {
                throw new Exception("Was not able to create user");
            }
            User user = new User(userId, createResponse.Value);
            Store.Add(user);
            return user;
        }

        public User DeleteUser(string userId) => DeleteUserAsync(userId).Result;
        public async Task<User> DeleteUserAsync(string userId)
        {
            var user = Store.Get(userId);
            var response = await Client.DeleteUserAsync(user.CommunicationUser);
            if (Utils.IsFailure(response))
            {
                throw new Exception("Was not able to delete user");
            }
            Store.Remove(userId, out var deletedUser);
            return deletedUser;
        }

        public User RefreshToken(string userId) => RefreshTokenAsync(userId).Result;
        public async Task<User> RefreshTokenAsync(string userId)
        {
            var user = Store.Get(userId);
            var response = await Client.IssueTokenAsync(user.CommunicationUser, Scope);
            if (Utils.IsFailure(response))
            {
                throw new Exception("Access tokens not issued");
            }
            User updatedUser = new User(userId, response);
            Store.Update(userId, updatedUser);
            return updatedUser;
        }

        public User RevokeAccessToken(string userId)
            => RevokeAccessTokenAsync(userId).Result;
        public async Task<User> RevokeAccessTokenAsync(string userId)
        {
            var user = Store.Get(userId);
            var response = await Client.RevokeTokensAsync(user.CommunicationUser);
            if (Utils.IsFailure(response))
            {
                throw new Exception("Access tokens not revoked");
            }
            return user;
        }
    }
}
