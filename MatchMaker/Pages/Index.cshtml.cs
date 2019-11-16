using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchMaker.Data;
using MatchMaker.Presentation;
using MatchMaker.Repositories;
using MatchMaker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Areas.MM.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserRepository userRepository;

        public List<UserModel> Users { get; set; }
        private readonly User CurrentDbUser;
        public UserModel CurrentUser { get; }

        public IndexModel(UserRepository userRepository, UserProvider userProvider)
        {
            this.CurrentDbUser = userProvider.DbUser;
            this.CurrentUser = userProvider.User;
            this.userRepository = userRepository;
        }
        public void OnGet()
        {
            var users = userRepository.GetDbSet().ToList();
            if(!this.CurrentUser.Name.StartsWith("Compu"))
            {
                users = users.Where(c => !((c.UserName.StartsWith("Compu") && c.UserName != "Compu")||c.UserName=="Werner")).ToList();
            }
            this.Users = users.Select(c => new UserModel(c, this.CurrentDbUser)).ToList();
        }
    }
}