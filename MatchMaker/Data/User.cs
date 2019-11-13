using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Data
{
    public class User : IdentityUser<Guid>
    {
        public User() : base()
        {

        }
        public string Name { get; set; }

        public string Alias { get; set; }

        public List<Ranking> ReceivedRankings { get => receivedRankings; set => this.SetRankings(value); }

        private void SetRankings(List<Ranking> value)
        {
            if (value != null)
            {
                this.receivedRankings = value;
                if (this.receivedRankings.Count(c => c.VotingPlayer.IsTrusted) > 2)
                {
                    this.IsTrusted = true;
                }
            }
        }

        public List<Ranking> GivenRankings { get; set; }

        public bool IsTrusted;
        private List<Ranking> receivedRankings;

        public void UpdateUserRanking(User votingUser, int ranking)
        {
            if(votingUser.Id == this.Id)
            { // We cannot vote for ourself.
                return;
            }
            var currentRanking = this.ReceivedRankings.SingleOrDefault(c => c.VotingPlayer.Id == votingUser.Id);
            if (currentRanking == null && ranking != 0)
            {
                currentRanking = new Ranking()
                {
                    AffectedPlayer = this,
                    VotingPlayer = votingUser
                };
                this.ReceivedRankings.Add(currentRanking);
                votingUser.GivenRankings.Add(currentRanking);
            } else if(ranking == 0)
            {
                this.ReceivedRankings.Remove(currentRanking);
                votingUser.GivenRankings.Remove(currentRanking);
                return;
            }
            currentRanking.Score = ranking;
            this.SetRankings(this.receivedRankings);
        }
    }

}
