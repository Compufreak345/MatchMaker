using MatchMaker.Data;
using MatchMaker.Services.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Services
{
    public class VoteCache
    {
        private readonly IMemoryCache cache;
        private object votingLock = new object();
        private ConcurrentDictionary<Guid, bool> activeVotes = new ConcurrentDictionary<Guid, bool>();

        public VoteCache(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public void AddVote(MapVote mapVote)
        {
            this.cache.Set(this.GetMapVoteKey(mapVote.Id), mapVote, TimeSpan.FromMinutes(30));
            activeVotes.TryAdd(mapVote.Id, true);
        }

        public async Task<MapVote> VoteAsync(Guid voteId, Guid mapId, User user)
        {
            var mapVote = this.cache.Get<MapVote>(this.GetMapVoteKey(voteId));
            lock (votingLock)
            {
                if (mapVote == null)
                {
                    return mapVote;
                }
                if (mapVote.PlayersVoted.Any(c => c == user.Id))
                {
                    return mapVote;
                }
                mapVote.PlayersVoted.Add(user.Id);
                mapVote.Maps.Single(c => c.Id == mapId).VoteCount++;
            }
            return mapVote;
        }

        public async Task<bool> DeleteVoteAsync(Guid voteId, User user)
        {
            var key = this.GetMapVoteKey(voteId);
            var mapVote = this.cache.Get<MapVote>(key);
            if (mapVote == null) return true;
            if(user.UserName != mapVote.StartingUser)
            {
                return false;
            }

            activeVotes.Remove(voteId, out _);
            this.cache.Remove(key);
            return true;
        }

            public IEnumerable<MapVote> GetActiveVotes()
        {
            List<MapVote> votes = new List<MapVote>();
            foreach (var item in activeVotes.Keys.ToList())
            {
                var key = this.GetMapVoteKey(item);
                this.cache.TryGetValue<MapVote>(key, out var vote);
                if (vote == null)
                { // Vote timed out.
                    activeVotes.Remove(item, out _);
                    continue;
                }
                votes.Add(vote);

            }
            return votes;
        }

        public string GetMapVoteKey(Guid mapVoteId)
        {
            return $"mapVote__{mapVoteId}";
        }
    }
}
