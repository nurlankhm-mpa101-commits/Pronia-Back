namespace Pronia.Extensions
{
    public static class FileExtensions
    {
        public static bool IsImage(this IFormFile file)
        {
            return file.ContentType.Contains("image");
        }

        public static bool LessThan(this IFormFile file, int mb)
        {
            return file.Length <= mb * 1024 * 1024;
        }
    }
}