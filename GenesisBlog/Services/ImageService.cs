using GenesisBlog.Services.Interfaces;

namespace GenesisBlog.Services
{
    public class ImageService : IImageService
    {

        public string ConvertByteArrayToFile(byte[] imageData, string ext)
        {
            var fileData = Convert.ToBase64String(imageData);
            var srcString = $"data:{ext};base64,{fileData}";
            return srcString;
        }

        public string ConvertByteArrayToFile(string userImageData, string? imageType)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
