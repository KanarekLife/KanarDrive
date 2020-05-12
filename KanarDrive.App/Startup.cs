using KanarDrive.App.Utils;
using KanarDrive.Common.Entities;
using KanarDrive.Common.Entities.Identity;
using KanarDrive.Common.FilesService;
using LiteDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KanarDrive.App
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILiteDatabase>(
                new LiteDatabase(_configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, Role>()
                .AddRoleStore<LiteDbRoleStore>()
                .AddUserStore<LiteDbUserStore>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            });
            services.AddScoped<DbUtils>();
            services.AddSingleton<FilesService>();
            services.AddSingleton<FilesService>();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DbUtils dbUtils)
        {
            dbUtils.Seed();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}