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

        public List<Ranking> ReceivedRankings { get; set; }

        public List<Ranking> GivenRankings { get; set; }

        public bool IsTrusted;
    }

}
