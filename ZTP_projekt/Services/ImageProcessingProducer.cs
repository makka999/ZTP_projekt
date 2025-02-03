using MassTransit;
using System.IO;
using System.Threading.Tasks;
using ZTP_projekt.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;

namespace ZTP_projekt.Services
{
    public class ImageProcessingProducer
    {
        private readonly IBus _bus;

        public ImageProcessingProducer(IBus bus)
        {
            _bus = bus;
        }

        public async Task SendImageProcessingRequest(string imagePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(imagePath);

            string leftBase64, rightBase64;
            using (var image = Image.Load(imagePath))
            {
                int halfWidth = image.Width / 2;

                using var leftPart = image.Clone(x => x.Crop(new Rectangle(0, 0, halfWidth, image.Height)));
                using var rightPart = image.Clone(x => x.Crop(new Rectangle(halfWidth, 0, halfWidth, image.Height)));

                leftBase64 = ConvertToBase64(leftPart);
                rightBase64 = ConvertToBase64(rightPart);
            }

            // Wysłanie dwóch wiadomości do RabbitMQ
            await _bus.Publish(new ImageProcessingCommand { ImageData = leftBase64, Part = "left", FileName = fileName });
            await _bus.Publish(new ImageProcessingCommand { ImageData = rightBase64, Part = "right", FileName = fileName });
        }

        private string ConvertToBase64(Image image)
        {
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
