using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Presentation
{
    public class RankingUpdateResponse
    {
        public UserModel VotedUser { get; set; }
        public UserModel VotingUser { get; set; }
    }
}
