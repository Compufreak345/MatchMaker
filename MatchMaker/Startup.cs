using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MatchMaker.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using System.IO;
using Microsoft.AspNetCore.Identity.UI.Services;
using MatchMaker.Services;
using MatchMaker.Configuration;
using Microsoft.AspNetCore.Http;
using MatchMaker.Middleware;
using MatchMaker.Repositories;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using MatchMaker.Hubs;

namespace MatchMaker
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var builder = new SqliteConnectionStringBuilder(connectionString);
            if (builder.DataSource.StartsWith("./"))
            { // Calculate relative path https://stackoverflow.com/questions/49290182/efcore-sqlite-connection-string-with-relative-path-in-asp-net
                // 
                builder.DataSource = Path.GetFullPath(
                    Path.Combine(
                        AppDomain.CurrentDomain.GetData("DataDirectory") as string
                            ?? AppDomain.CurrentDomain.BaseDirectory,
                        builder.DataSource));
                connectionString = builder.ToString();
            }

            services.AddSignalR();
            services.AddDbContext<MmDbContext>(options =>
                options.UseSqlite(
                    connectionString));
            services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 30;
            })
                .AddEntityFrameworkStores<MmDbContext>()
                ;
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<UserRepository>();
            services.AddScoped<MatchCreator>();
            services.AddScoped<UserProvider>();
            services.AddTransient<IEmailSender, MmEmailSender>();
            services.Configure<Email>(this.Configuration.GetSection("Email"));
            services.AddSingleton<VoteCache>();

            services.AddRazorPages();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            if (!this.env.IsDevelopment())
            {
                services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(@"/home/matchmaker/database/keys.txt"));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MmDbContext dbContext)
        {
            dbContext.Database.Migrate();
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {

                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<UserProviderMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notificationHub");

            });
        }
    }
}
