using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace ZTP_projekt.Services
{
    public static class ImageProcessingService
    {
        public static string DarkenImage(string base64Image, float brightnessFactor = 0.5f)
        {
            try
            {
                using var image = LoadFromBase64(base64Image);
                image.Mutate(x => x.Brightness(brightnessFactor));

                return ConvertToBase64(image);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Błąd przyciemiania obrazu: {ex.Message}");
                return null;
            }
        }

        private static Image LoadFromBase64(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            return Image.Load(imageBytes);
        }

        private static string ConvertToBase64(Image image)
        {
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
