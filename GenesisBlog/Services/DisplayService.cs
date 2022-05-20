using GenesisBlog.Data;
using GenesisBlog.Services.Interfaces;

namespace GenesisBlog.Services
{
    public class DisplayService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageService _imageService;

        public DisplayService(ApplicationDbContext dbContext, IImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }

        public async Task<string> DisplayModName(string id)
        {
            var moderator = await _dbContext.Users.FindAsync(id);
            return moderator?.FullName!;
            
        }

        public async Task<string> DisplayAuthorImage(string id)
        {
            string defaultImageSource = "https://cdn.icon-icons.com/icons2/233/PNG/512/Contacts_26253.png";
            var author = await _dbContext.Users.FindAsync(id);

            return author!.ImageData is not null ? _imageService.ConvertByteArrayToFile(author.ImageData, author.ImageType!) : defaultImageSource;
        }
    }

}
