using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeBlog.Data;
using WeBlog.Enums;
using WeBlog.Models;

namespace WeBlog.Services
{
    public class DataService
    {
        /*
            Purposes:
            
            1- Seeding Roles into the system
            2- Seeding Users into the system

         */

        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            // If there are already roles in the system, do not seed.
            if (_dbContext.Roles.Any())
            {
                return;
            }

            // Otherwise, seed roles.
            foreach(var role in Enum.GetNames(typeof(BlogRole)))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            // If there are already users in the system, do not seed.
            if (_dbContext.Users.Any())
            {
                return;
            }

            var adminUserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "admin@weblog.com";
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Adminpassword123!";

            // Otherwise, seed users.
            BlogUser adminUser = new BlogUser()
            {
                Email = "admin@weblog.com",
                UserName =  adminUserName,
                FirstName = "John",
                LastName = "Doe",
                DisplayName = "Admin",
                PhoneNumber = "(612) 111-2222",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(adminUser, adminPassword);
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            var modUserName = Environment.GetEnvironmentVariable("MOD_USERNAME") ?? "mod@weblog.com";
            var modPassword = Environment.GetEnvironmentVariable("MOD_PASSWORD") ?? "Modpassword123!";

            BlogUser modUser = new BlogUser()
            {
                Email = "moderator@weblog.com",
                UserName = modUserName,
                FirstName = "John",
                LastName = "Doe",
                DisplayName = "Moderator",
                PhoneNumber = "(612) 111-3333",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(modUser, modPassword);
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }
    }
}
