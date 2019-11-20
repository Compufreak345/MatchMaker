using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Services.Models
{
    public class VoteItem
    {
        public string Name { get; set; }
        public int VoteCount { get; set; }
        public Guid Id { get; set; }
    }
}
