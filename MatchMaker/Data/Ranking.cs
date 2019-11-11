using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Data
{
    public class Ranking
    {
        public Guid Id { get; set; }
        [Range(0,10)]
        public int Score { get; set; }
        public User AffectedPlayer { get; set; }
        public User VotingPlayer { get; set; }
    }
}
