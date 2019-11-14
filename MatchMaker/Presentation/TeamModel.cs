using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Presentation
{
    public class TeamModel
    {
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public string Name { get; set; }

        public double Ranking => this.Users.Sum(c => c.GetFallbackRanking());
    }
}
