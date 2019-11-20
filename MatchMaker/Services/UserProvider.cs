using MatchMaker.Data;
using MatchMaker.Presentation;
using MatchMaker.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MatchMaker.Services
{
    public class UserProvider
    {
        public UserModel User => new UserModel(DbUser);

        public User DbUser { get; set; }


        public UserProvider()
        {
        }
        public Task SetUserByHttpContextAsync(HttpContext context, UserRepository userRepository)
        {
            return this.SetUserByIdAsync(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, userRepository);
        }

        public async Task SetUserByIdAsync(string userId, UserRepository userRepository)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var user = await userRepository.GetUserAsync(Guid.Parse(userId));
            this.DbUser = user;
        }
    }
}
