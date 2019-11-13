using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchMaker.Data;
using MatchMaker.Presentation;
using MatchMaker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Areas.MM.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MmDbContext dbContext;
        public List<UserModel> Users { get; set; }

        private readonly User currentDbUser;

        public UserModel CurrentUser { get; }

        public IndexModel(MmDbContext dbContext, UserProvider userProvider)
        {
            this.dbContext = dbContext;
            this.currentDbUser = userProvider.User;
            this.CurrentUser = new UserModel(this.currentDbUser);
        }
        public void OnGet()
        {
            var users = dbContext.Users.Include(c => c.ReceivedRankings).Include(c => c.GivenRankings).ToList();
            this.Users = users.Select(c => new UserModel(c)).ToList();
        }

        public async Task<ActionResult> OnPostAsync(Guid userId, int ranking)
        {
            var user = dbContext.Users.Include(c => c.ReceivedRankings).Include(c => c.GivenRankings).ThenInclude(c => c.VotingPlayer).Single(c => c.Id == userId);
            if (user == null)
            {
                return BadRequest();
            }
            user.UpdateUserRanking(this.currentDbUser, ranking);
            await this.dbContext.SaveChangesAsync();
            var result = new RankingUpdateResponse()
            {
                VotedUser = new UserModel(user),
                VotingUser = new UserModel(this.currentDbUser)
            };
            return new ObjectResult(result);
        }
    }
}