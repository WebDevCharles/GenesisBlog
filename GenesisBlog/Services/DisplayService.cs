using GenesisBlog.Data;

namespace GenesisBlog.Services
{
    public class DisplayService
    {
        private readonly ApplicationDbContext _dbContext;

        public DisplayService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> DisplayModName(string id)
        {
            var moderator = await _dbContext.Users.FindAsync(id);
            return moderator?.FullName!;
            
        }
    }
}
