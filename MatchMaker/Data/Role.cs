using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Data
{
    public class Role : IdentityRole<Guid>
    {
        public Role(): base()
        {

        }
    }
}
