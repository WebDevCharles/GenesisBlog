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

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is null)
            {
                await _userManager.CreateAsync(user, "Purpl3Walru$!");
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            email = "JasonTwichell@mailinator.com";
            user = new()
            {
                UserName = email,
                Email = email,
                FirstName = "Jason",
                LastName = "Twichell",
                EmailConfirmed = true,
            };

            existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is null)
            {
                await _userManager.CreateAsync(user, "P1cklesRYummy!");
                await _userManager.AddToRoleAsync(user, "Moderator");
            }

        }

        private async Task SeedTagsAsync()
        {
            if (_dbContext.Tag.Count() == 0)
            {
                await _dbContext.AddAsync(new Tag() { Text = "Role Based Security" });
                await _dbContext.AddAsync(new Tag() { Text = "Scaffolding Against A Model" });
                await _dbContext.AddAsync(new Tag() { Text = "Customizing Identity" });
                await _dbContext.AddAsync(new Tag() { Text = "Working With Interfaces" });
                await _dbContext.SaveChangesAsync();
            }
        }




    }
}
