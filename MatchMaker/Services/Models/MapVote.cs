using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Services.Models
{
    public class MapVote
    {
        public VoteItem[] Maps { get; set; }
        public List<Guid> PlayersVoted { get; set; }
        public DateTimeOffset VoteStart { get; set; }
        public Guid Id { get; set; }
        public string StartingUser { get; set; }
    }
}
