using System.Linq;
using KanarDrive.Common.Entities.Identity;
using KanarDrive.Common.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KanarDrive.App.Utils
{
    public class DbUtils
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Startup> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public DbUtils(ILogger<Startup> logger, RoleManager<Role> roleManager, UserManager<User> userManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public void Seed()
        {
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new Role
                {
                    Name = "Admin"
                }).GetAwaiter().GetResult();
                _logger.LogInformation("Created Admin role!");
            }

            if (!_userManager.Users.Any() || !_userManager.GetUsersInRoleAsync("Admin").GetAwaiter().GetResult().Any())
            {
                var defaultAdmin = new DefaultUser();
                _configuration.Bind("DefaultAdmin", defaultAdmin);
                var user = new User
                {
                    UserName = defaultAdmin.Username,
                    Email = defaultAdmin.Email,
                    Firstname = defaultAdmin.Firstname,
                    Lastname = defaultAdmin.Lastname,
                    AvailableCloudSpace = defaultAdmin.AvailableCloudSpaceInGbs
                };
                _userManager.CreateAsync(user, defaultAdmin.Password).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
                _logger.LogInformation("Created new admin!");
            }
        }
    }
}