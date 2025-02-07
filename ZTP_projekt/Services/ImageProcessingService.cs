using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ZTP_projekt.Services
{
    public static class ImageProcessingService
    {
        public static Bitmap ApplyBrightness(Bitmap bitmap, float brightnessFactor)
        {
            Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color oldColor = bitmap.GetPixel(x, y);
                    int r = (int)(brightnessFactor * oldColor.R);
                    int g = (int)(brightnessFactor * oldColor.G);
                    int b = (int)(brightnessFactor * oldColor.B);
                    bitmap2.SetPixel(x, y, Color.FromArgb(r, g, b));
                }

            return bitmap2;
        }
    }
}
