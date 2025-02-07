using MassTransit;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZTP_projekt.Models;
using System;

namespace ZTP_projekt.Services
{
    public class ImageProcessingProducer
    {
        private readonly IBus _bus;
        private readonly string _inputFolder = Path.Combine("wwwroot", "ProcessedImages");

        public ImageProcessingProducer(IBus bus)
        {
            _bus = bus;
        }

        // 🔥 Metoda, której brakowało
        public async Task SendImageProcessingRequest(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"[ERROR] Plik {imagePath} nie istnieje.");
                return;
            }

            Console.WriteLine($"[INFO] Wysyłanie obrazu {Path.GetFileName(imagePath)} do kolejki...");

            await _bus.Publish(new ImageProcessingCommand
            {
                ImagePath = imagePath,
                FileName = Path.GetFileName(imagePath)
            });
        }

        // 🔥 Metoda do przetwarzania wszystkich obrazów w folderze
        public async Task SendImageProcessingRequests()
        {
            if (!Directory.Exists(_inputFolder))
            {
                Console.WriteLine("[ERROR] Folder wejściowy nie istnieje.");
                return;
            }

            var imagePaths = Directory.GetFiles(_inputFolder, "*.jpg");
            if (!imagePaths.Any())
            {
                Console.WriteLine("[INFO] Brak obrazów do przetworzenia.");
                return;
            }

            foreach (var imagePath in imagePaths)
            {
                await SendImageProcessingRequest(imagePath);
            }
        }
    }
}
