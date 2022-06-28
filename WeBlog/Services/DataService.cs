using Microsoft.AspNetCore.Identity;
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

            // Otherwise, seed users.
            BlogUser adminUser = new BlogUser()
            {
                Email = "alex.monteil@outlook.com",
                UserName = "alex.monteil@outlook.com",
                FirstName = "Alex",
                LastName = "Monteil",
                PhoneNumber = "(612) 111-2222",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(adminUser, "Adminpassword123!");

            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            BlogUser modUser = new BlogUser()
            {
                Email = "moderator@weblog.com",
                UserName = "moderator@weblog.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "(612) 111-3333",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(modUser, "Modpassword123!");
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }
    }
}
