using Azure.Communication.Administration;
using ChatModule.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ChatModule.Test")]
namespace ChatModule.Services
{
    public sealed class UserService : BaseService<User, CommunicationIdentityClient>
    {
        private readonly List<CommunicationTokenScope> Scope =
            new List<CommunicationTokenScope>() { CommunicationTokenScope.Chat };
        public UserService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            try
            {
                Client = new CommunicationIdentityClient(AccessTokens.ConnectionString);
            } catch(Exception e)
            {
                if(AccessTokens.SecondaryConnectionString != null)
                {
                    try
                    {
                        Client = new CommunicationIdentityClient(AccessTokens.SecondaryConnectionString);
                    }
                    catch (Exception eInner)
                    {
                        throw eInner;
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        public UserService(IServiceProvider serviceProvider, IEnumerable<CommunicationTokenScope> scopes)
            : this(serviceProvider)
        {
            Scope.AddRange(scopes);
        }

        public User CreateUser(string userId) => CreateUserAsync(userId).Result;
        public async Task<User> CreateUserAsync(string userId)
        {
            if (Store.Exists(userId))
            {
                throw new Exception("User ID Already exists");
            }
            User user = new User(userId, await Client.IssueTokenAsync(await Client.CreateUserAsync(), Scope));
            if (!Store.Add(user))
            {
                throw new Exception("Unable to create user");
            }
            return user;
        }

        public Azure.Response DeleteUser(string userId) => DeleteUserAsync(userId).Result;
        public async Task<Azure.Response> DeleteUserAsync(string userId)
        {
            var user = Store.Get(userId);
            var response = await Client.DeleteUserAsync(user.CommunicationUser);
            if (Utils.IsSuccess(response) && Store.Remove(userId, out _))
            {
                return response;
            }
            throw new Exception("User not Deleted");
        }

        public User RefreshToken(string userId) => RefreshTokenAsync(userId).Result;
        public async Task<User> RefreshTokenAsync(string userId)
        {
            var user = Store.Get(userId);
            var azureResponse = await Client.IssueTokenAsync(user.CommunicationUser, Scope);
            User updatedUser = new User(userId, azureResponse);
            Store.Update(userId, user, updatedUser);
            return user;
        }

        public Azure.Response RevokeAccessToken(string userId) => RevokeAccessTokenAsync(userId).Result;
        public async Task<Azure.Response> RevokeAccessTokenAsync(string userId)
        {
            var user = Store.Get(userId);
            var response = await Client.RevokeTokensAsync(user.CommunicationUser);
            if (Utils.IsFailure(response))
            {
                throw new Exception("Access tokens not revoked");
            }
            return response;
        }
    }
}
