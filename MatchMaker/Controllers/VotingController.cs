using MatchMaker.Data;
using MatchMaker.Presentation;
using MatchMaker.Repositories;
using MatchMaker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class VotingController
    {
        private readonly User currentDbUser;
        private readonly UserRepository userRepository;

        public VotingController(UserRepository userRepository, UserProvider userProvider)
        {
            this.currentDbUser = userProvider.DbUser;
            this.userRepository = userRepository;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Vote([FromForm] Guid userId, [FromForm] int ranking)
        {
            var user = await this.userRepository.GetUserAsync(userId);
            if (user == null)
            {
                return new BadRequestResult();
            }
            user.UpdateUserRanking(this.currentDbUser, ranking);
            await this.userRepository.SaveChangesAsync();
            var result = new RankingUpdateResponse()
            {
                VotedUser = new UserModel(user, this.currentDbUser),
                VotingUser = new UserModel(this.currentDbUser)
            };
            return new OkObjectResult(result);
        }
    }
}
