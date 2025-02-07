using MassTransit;
using ZTP_projekt.Models;
using ZTP_projekt.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ZTP_projekt.Services
{
    public class ImageProcessingConsumer : IConsumer<ImageProcessingCommand>
    {
        private readonly string _outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProcessedImages", "Dark");

        public async Task Consume(ConsumeContext<ImageProcessingCommand> context)
        {
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), context.Message.ImagePath);
            string outputPath = Path.Combine(_outputFolder, context.Message.FileName);

            Console.WriteLine($"[INFO] Odbieranie obrazu: {imagePath}");
            Console.WriteLine($"[INFO] Zapis do: {outputPath}");

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"[ERROR] Plik {imagePath} nie istnieje.");
                return;
            }

            Directory.CreateDirectory(_outputFolder); // 🔥 Tworzenie folderu, jeśli nie istnieje

            try
            {
                using var bitmap = new Bitmap(imagePath);
                using var darkenedBitmap = ImageProcessingService.ApplyBrightness(bitmap, 0.75f);
                darkenedBitmap.Save(outputPath, ImageFormat.Jpeg);

                Console.WriteLine($"[INFO] Zapisano przetworzony obraz: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Błąd przetwarzania obrazu: {ex.Message}");
            }
        }
    }
}
