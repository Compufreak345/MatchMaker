using MatchMaker.Data;
using MatchMaker.Presentation;
using MatchMaker.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Services
{
    public class MatchCreator
    {
        Random rand = new Random();
        List<string> TeamNames = new List<string>
        {
            "Elefanten",
            "Pferde",
            "Fische",
            "Schweine",
            "Ferkel",
            "Mäuse",
            "Pfirsiche",
            "Krähen",
            "Raben",
            "Eichhörnchen",
            "Tiger",
            "Gazellen",
            "Geparde",
            "Tintenfischringe"
        };
        private readonly UserRepository userRepository;

        public MatchCreator(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<TeamModel>> MakeMatchAsync(IEnumerable<Guid> userIds, int noOfTeams, double allowedAbweichung = 0.1, int tryCount = 0)
        {
            tryCount++;
            Shuffle(TeamNames);
            var teams = new List<TeamModel>();
            for (int i = 0; i < noOfTeams; i++)
            {
                teams.Add(new TeamModel()
                {
                    Name = TeamNames.ElementAt(i)
                });
            }

            var players = (await this.userRepository.GetUsersAsync(userIds)).Select(c => new UserModel(c));
            var playerCount = players.Count();
            var rankSum = players.Select(c => c.GetFallbackRanking()).Sum();
            var targetSumPerTeam = rankSum / noOfTeams;
            var minSumPerTeam = targetSumPerTeam - targetSumPerTeam * allowedAbweichung / 2;
            var maxSumPerTeam = targetSumPerTeam + targetSumPerTeam * allowedAbweichung / 2;
            int playersPerTeam = playerCount / noOfTeams + playerCount % noOfTeams;

            foreach (var player in players)
            {
                int teamFindTryCount = 0;
                int teamIdx = 0;
                TeamModel team;
                do
                {
                    teamIdx = rand.Next(noOfTeams);
                    team = teams.ElementAt(teamIdx);
                    teamFindTryCount++;
                } while (teamFindTryCount < 100 && !this.TeamIsInRange(maxSumPerTeam, playersPerTeam, team, player));
                team.Users.Add(player);
            }


            if (teams.Any(c => !this.TeamIsInRange(maxSumPerTeam, playersPerTeam, c, minSumPerTeam: minSumPerTeam)))
            {
                if (tryCount > 100)
                {
                    allowedAbweichung += 0.025;
                    tryCount = 0;
                }
                return await this.MakeMatchAsync(userIds, noOfTeams, allowedAbweichung, tryCount);
            }
            return teams;
        }

        private bool TeamIsInRange(double maxSumPerTeam, int playersPerTeam, TeamModel team, UserModel additionalPlayer = null, double minSumPerTeam = 0)
        {
            var players = team.Users.Select(c=>c).ToList();
            if(additionalPlayer != null)
            {
                players.Add(additionalPlayer);
            }

            if (players.Count > playersPerTeam)
            {
                return false;
            }

            if (team.Ranking > maxSumPerTeam || team.Ranking < minSumPerTeam)
            {
                return false;
            }

            return true;
        }

        // https://stackoverflow.com/questions/273313/randomize-a-listt
        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
