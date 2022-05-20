namespace GenesisBlog.Services.Interfaces
{
    public interface IImageService
    {
        Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);
        string ConvertByteArrayToFile(byte[] imageData, string extension);
        string ConvertByteArrayToFile(string userImageData, string? imageType);
    }
}
