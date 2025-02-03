using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace ZTP_projekt.Utils
{
    public static class ImageSplitter
    {
        public static void SplitImage(string originalPath, string leftPath, string rightPath)
        {
            using var image = Image.Load(originalPath);
            int halfWidth = image.Width / 2;

            using var leftPart = image.Clone(x => x.Crop(new Rectangle(0, 0, halfWidth, image.Height)));
            using var rightPart = image.Clone(x => x.Crop(new Rectangle(halfWidth, 0, halfWidth, image.Height)));

            leftPart.Save(leftPath);
            rightPart.Save(rightPath);
        }
    }
}
