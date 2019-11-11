using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Data
{
    public class MmDbContext : IdentityDbContext<User, Role, Guid>
    {
        public MmDbContext(DbContextOptions<MmDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().HasMany(c => c.GivenRankings)
                .WithOne(c => c.VotingPlayer);
            builder.Entity<User>().HasMany(c => c.ReceivedRankings)
                .WithOne(c => c.AffectedPlayer);


        }
    }
}
