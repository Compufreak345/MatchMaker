using MatchMaker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Presentation
{
    public class UserModel
    {
        public UserModel()
        {

        }

        public UserModel(User user)
        {

            this.Name = user.UserName;
            this.PersonalRanking = this.CalcUserRanking(user.ReceivedRankings.ToList());
            this.VotesGiven = user.GivenRankings?.Count() ?? 0;
            this.VotesReceived = user.ReceivedRankings?.Count() ?? 0;
            this.IsTrusted = user.IsTrusted;
            this.Id = user.Id;
        }

        public string Name { get; set; }

        public int PersonalRanking { get; set; }

        public int VotesReceived { get; set; }

        public int VotesGiven { get; set; }

        public bool IsTrusted { get; set; }

        public Guid Id { get; set; }

        public int CalcUserRanking(List<Ranking> receivedRankings)
        {
            if (!receivedRankings.Any())
            {
                return 0;
            }
            return receivedRankings.Sum(c => c.Score) / receivedRankings.Count();
        }
    }
}
