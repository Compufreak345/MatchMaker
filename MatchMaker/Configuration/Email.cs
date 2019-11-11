using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Configuration
{
    public class Email
    {
        public string Server { get; set; }
        public string Password { get; set; }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
        public string UserName { get; set; }
        public int Port { get; set; }
    }
}
