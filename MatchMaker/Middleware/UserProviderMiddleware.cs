using MatchMaker.Data;
using MatchMaker.Repositories;
using MatchMaker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        public async Task InvokeAsync(HttpContext context, UserProvider userProvider, UserRepository userRepository, SignInManager<User> signInManager)
        {
            if (context.User != null)
            {
                await userProvider.SetUserByHttpContextAsync(context, userRepository);

                if(userProvider.DbUser == null && context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
                {
                    context.Response.Clear();
                    foreach (var cookie in context.Request.Cookies)
                    {
                        context.Response.Cookies.Delete(cookie.Key);
                    }
                    context.Response.Redirect("/Identity/Account/Login");
                    await signInManager.SignOutAsync();
                }
            }

            // Call the next delegate/middleware in the pipeline
            await this.next(context);
        }
    }
}
