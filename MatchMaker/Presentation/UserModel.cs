using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Presentation
{
    public class UserModel
    {
        public string Name { get; set; }

        public int Ranking { get; set; }

        public int VotesReceived { get; set; }

        public int VotesGiven { get; set; }

        public bool IsTrusted { get; set; }
    }
}
