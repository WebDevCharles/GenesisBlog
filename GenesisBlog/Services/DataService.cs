using GenesisBlog.Data;
using GenesisBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GenesisBlog.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SetupDBAsync()
        {
            //Run a single command to execute all of the migrations
            await _dbContext.Database.MigrateAsync();

            //Call a private method that adds a few Roles into the system
            //Adds 2 records into the AspNetRoles table
            await SeedRolesAsync();
            await SeedUsersAsync();

        }

        private async Task SeedRolesAsync()
        {
            //This runs every time the application is started therefore I want
            //to make sure nothing is done if there are ANY records in the Roles table.
            if (_roleManager.Roles.Count() == 0)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Moderator"));
            }

        }

        private async Task SeedUsersAsync()
        {
            //Create a new instance of Blog User
            string email = "productofva@gmail.com";
            BlogUser user = new()
            {
                UserName = email,
                Email = email,
                FirstName = "Charles",
                LastName = "Hall",
                EmailConfirmed = true,
            };

            var newUser = await _userManager.FindByEmailAsync(email);
            if(newUser == null)
            {
                await _userManager.CreateAsync(user, "Blu3j3ll0!");
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            string emailModerator = "JasonTwichell@mailinator.com";
            BlogUser userModerator = new()
            {
                UserName = emailModerator,
                Email = emailModerator,
                FirstName = "Jason",
                LastName = "Twichell",
                EmailConfirmed = true,
            };

            var newUserModerator = await _userManager.FindByEmailAsync(emailModerator);
            if (newUserModerator == null)
            {
                await _userManager.CreateAsync(user, "PicklesRYummy!");
                await _userManager.AddToRoleAsync(user, "Moderator");
            }

        }




    }
}
