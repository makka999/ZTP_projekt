using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace ZTP_projekt.Utils
{
    public static class ImageMerger
    {
        public static void MergeImages(string leftPath, string rightPath, string finalPath)
        {
            using var leftImage = Image.Load(leftPath);
            using var rightImage = Image.Load(rightPath);

            int width = leftImage.Width + rightImage.Width;
            int height = leftImage.Height;

            using var finalImage = new Image<Rgba32>(width, height);
            finalImage.Mutate(ctx =>
            {
                ctx.DrawImage(leftImage, new Point(0, 0), 1f);
                ctx.DrawImage(rightImage, new Point(leftImage.Width, 0), 1f);
            });

            finalImage.Save(finalPath);
        }
    }
}
