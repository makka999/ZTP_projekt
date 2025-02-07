namespace ZTP_projekt.Models
{
    public class ImageProcessingCommand
    {
        public string ImagePath { get; set; }
        public string Part { get; set; }
        public string FileName { get; set; }
        public string ImageData { get; set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
    }
}
