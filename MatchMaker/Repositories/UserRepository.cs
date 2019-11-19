using MatchMaker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Repositories
{
    public class UserRepository
    {
        private readonly MmDbContext dbContext;

        public UserRepository(MmDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            dbContext.Database.ExecuteSqlRaw("PRAGMA synchronous = OFF");
            return this.dbContext.Database.BeginTransactionAsync();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            return await this.GetDbSet().SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<User>> GetUsersAsync(IEnumerable<Guid> ids)
        {
            return await this.GetDbSet().Where(c => ids.Contains(c.Id)).ToListAsync();
        }

        public IQueryable<User> GetDbSet()
        {
           return this.dbContext.Users
                .Include(c => c.ReceivedRankings)
                    .ThenInclude(c => c.VotingPlayer)
                .Include(c => c.GivenRankings)
                    .ThenInclude(c => c.VotingPlayer);
        }

        internal Task SaveChangesAsync()
        {
            return this.dbContext.SaveChangesAsync();
        }
    }
}
