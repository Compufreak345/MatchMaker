using MatchMaker.Data;
using MatchMaker.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Services
{
    public class UserProvider
    {
        public UserModel User => new UserModel(DbUser);

        public User DbUser { get; set; }
    }
}
