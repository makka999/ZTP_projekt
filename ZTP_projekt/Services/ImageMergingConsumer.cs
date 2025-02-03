using MassTransit;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using ZTP_projekt.Models;
using System;
using SixLabors.ImageSharp.PixelFormats;

namespace ZTP_projekt.Services
{
    public class ImageMergingConsumer : IConsumer<ImageMergingCommand>
    {
        private static ConcurrentDictionary<string, (string Left, string Right)> processedImages = new();
        private readonly string _outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProcessedImages"); 

        public async Task Consume(ConsumeContext<ImageMergingCommand> context)
        {
            string key = context.Message.FileName;
            if (processedImages.TryGetValue(key, out var existingParts))
            {
                if (context.Message.Part == "left")
                    existingParts = (context.Message.ImageData, existingParts.Right);
                else
                    existingParts = (existingParts.Left, context.Message.ImageData);

                if (!string.IsNullOrEmpty(existingParts.Left) && !string.IsNullOrEmpty(existingParts.Right))
                {
                    Directory.CreateDirectory(_outputPath); 
                    string finalPath = Path.Combine(_outputPath, $"{key}_final.jpg");
                    MergeAndSave(existingParts.Left, existingParts.Right, finalPath);
                    processedImages.TryRemove(key, out _);
                }
                else
                {
                    processedImages[key] = existingParts;
                }
            }
            else
            {
                processedImages[key] = context.Message.Part == "left"
                    ? (context.Message.ImageData, null)
                    : (null, context.Message.ImageData);
            }
        }

        private void MergeAndSave(string leftBase64, string rightBase64, string finalPath)
        {
            using var leftImage = LoadFromBase64(leftBase64);
            using var rightImage = LoadFromBase64(rightBase64);
            int width = leftImage.Width + rightImage.Width;
            int height = leftImage.Height;

            using var finalImage = new Image<Rgba32>(width, height);
            finalImage.Mutate(ctx =>
            {
                ctx.DrawImage(leftImage, new Point(0, 0), 1f);
                ctx.DrawImage(rightImage, new Point(leftImage.Width, 0), 1f);
            });

            finalImage.Save(finalPath);
            Console.WriteLine($"[INFO] Final image saved at: {finalPath}");
        }

        private Image LoadFromBase64(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            return Image.Load(imageBytes);
        }
    }
}
