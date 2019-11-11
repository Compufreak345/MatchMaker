using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Data
{
    public class Game
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User> Players { get; set; }
    }
}
