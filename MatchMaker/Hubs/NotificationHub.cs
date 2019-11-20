using MatchMaker.Data;
using MatchMaker.Repositories;
using MatchMaker.Services;
using MatchMaker.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IServiceProvider serviceProvider;
        private readonly VoteCache cache;
        private readonly UserRepository userRepository;

        public NotificationHub(IServiceProvider serviceProvider, VoteCache cache, UserRepository userRepository)
        {
            this.serviceProvider = serviceProvider;
            this.cache = cache;
            this.userRepository = userRepository;
        }

        public async Task StartVote(string maps)
        {

            var user = await this.GetCurrentUserAsync();
            if(this.cache.GetActiveVotes().Count(c=>c.StartingUser == user.UserName) > 4)
            {
                await Clients.Caller.SendAsync("Alert", "Mehr als 5 offene Votes am Stück sind nicht erlaubt.");
                return;
            }

            var mapList = maps.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(c=> new VoteItem()
            {
                Name = c,
                VoteCount = 0,
                Id = Guid.NewGuid()
            }).ToArray();

            var mapVote = new MapVote()
            {
                Maps = mapList,
                VoteStart = DateTime.UtcNow,
                PlayersVoted = new List<Guid>(),
                Id = Guid.NewGuid(),
                StartingUser = user.UserName
            };

            this.cache.AddVote(mapVote);
            
            await Clients.All.SendAsync("ReceiveVoteStarted", mapVote, user.UserName);
        }

        public async override Task OnConnectedAsync()
        {
            
            await base.OnConnectedAsync();
            foreach(var vote in this.cache.GetActiveVotes().OrderBy(c=>c.VoteStart))
            {
                await Clients.Caller.SendAsync("ReceiveVoteStarted", vote, vote.StartingUser);
            }
            
        }

        public async Task Vote(Guid voteId, Guid mapId)
        {

            var user = await this.GetCurrentUserAsync();
            var vote = await this.cache.VoteAsync(voteId, mapId, user);

            await Clients.All.SendAsync("UpdateVote", vote);
        }


        private async Task<User> GetCurrentUserAsync()
        {
            using var scope = serviceProvider.CreateScope();

            var userProvider = serviceProvider.GetRequiredService<UserProvider>();
            var userRepository = serviceProvider.GetRequiredService<UserRepository>();
            await userProvider.SetUserByHttpContextAsync(this.Context.GetHttpContext(), userRepository);
            return userProvider.DbUser;
        }
    }
}
