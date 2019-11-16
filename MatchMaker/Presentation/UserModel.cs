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

        public UserModel(User user, User VotingUser = null)
        {
            this.Name = user.UserName;
            if(VotingUser != null)
            {
                this.YourGivenRanking = VotingUser.GivenRankings.SingleOrDefault(c=>c.AffectedPlayer?.Id == user.Id)?.Score ?? 0;
            }
            this.CalculatedRanking = this.CalcUserRanking(user.ReceivedRankings.ToList());
            this.VotesGiven = user.GivenRankings?.Count() ?? 0;
            this.VotesReceived = user.ReceivedRankings?.Count() ?? 0;
            this.IsTrusted = user.IsTrusted;
            this.Id = user.Id;
        }

        public int YourGivenRanking { get; set; }

        public string Name { get; set; }

        public double CalculatedRanking { get; set; }

        public int VotesReceived { get; set; }

        public int VotesGiven { get; set; }

        public bool IsTrusted { get; set; }

        public Guid Id { get; set; }

        private double CalcUserRanking(List<Ranking> receivedRankings)
        {
            var validRankings = receivedRankings.Where(c => c.VotingPlayer?.IsTrusted ?? false);
            if (!validRankings.Any())
            {
                return 0;
            }
            
            return Math.Round((double)(validRankings.Sum(c => c.Score)) / validRankings.Count(), 1);
        }

        /// <summary>
        /// Calculates ranking and returns 5 if no ranking was found.
        /// </summary>
        /// <returns></returns>
        public double GetFallbackRanking()
        {
                return this.CalculatedRanking == 0 ? 5 : this.CalculatedRanking;
            
        }
    }
}
