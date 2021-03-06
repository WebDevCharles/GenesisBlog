using GenesisBlog.Data;
using GenesisBlog.Models;
using GenesisBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GenesisBlog.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IImageService _imageService;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager, IImageService imageService)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _imageService = imageService;
        }

        public async Task SetupDBAsync()
        {
            //Run a single command to execute all of the migrations
            await _dbContext.Database.MigrateAsync();

            //Call a private method that adds a few Roles into the system
            //Adds 2 records into the AspNetRoles table
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedTagsAsync();

            await SeedTestBlogs();

        }

        private async Task SeedTestBlogs()
        {
            if (_dbContext.BlogPosts.Any())
                return;

            for (var loop = 1;loop <= 10;loop++)
            {
                _dbContext.Add(new BlogPost()
                {
                    Title = $"Blog Title {loop}: The importance of {loop}'s in coding...",
                    Abstract = $"Have you ever wondered why the number of {loop} plays such a significant role in coding?",
                    Content = "Test Content...",
                    Created = DateTime.UtcNow.AddDays(-loop),
                    Slug = $"blog-title-{loop}-the-importance-of-{loop}s-in-coding",
                });
            }
            await _dbContext.SaveChangesAsync();
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
