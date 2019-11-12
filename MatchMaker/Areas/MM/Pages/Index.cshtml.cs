using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchMaker.Data;
using MatchMaker.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Areas.MM.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MmDbContext dbContext;
        public List<UserModel> Users { get; set; }
        public IndexModel(MmDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet()
        {
            var users = dbContext.Users.Include(c=>c.ReceivedRankings).Include(c=>c.GivenRankings).ToList();
            this.Users = users.Select(c => new UserModel()
            {
                Name = c.UserName,
                Ranking = this.CalcUserRanking(c.ReceivedRankings.ToList()),
                VotesGiven = c.GivenRankings.Count(),
                VotesReceived = c.ReceivedRankings.Count(),
                IsTrusted = c.IsTrusted
            }).ToList();
        }

        private int CalcUserRanking(List<Ranking> receivedRankings)
        {
            if(!receivedRankings.Any())
            {
                return 0;
            }
            return receivedRankings.Sum(c => c.Score) / receivedRankings.Count();
        }
    }
}