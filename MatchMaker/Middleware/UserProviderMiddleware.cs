using MatchMaker.Data;
using MatchMaker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MatchMaker.Middleware
{
    public class UserProviderMiddleware
    {
        private readonly RequestDelegate next;

        public UserProviderMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserProvider userProvider, MmDbContext dbContext)
        {
            if(context.User != null)
            {
                var user = dbContext.Users.Include(c=>c.ReceivedRankings).Include(c=>c.GivenRankings).Single(c => c.Id == Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value));
                userProvider.User = user;
            }

            // Call the next delegate/middleware in the pipeline
            await this.next(context);
        }
    }
}
