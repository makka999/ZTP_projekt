using MassTransit;
using ZTP_projekt.Models;
using ZTP_projekt.Services;
using System.Threading.Tasks;

namespace ZTP_projekt.Services
{
    public class ImageProcessingConsumer : IConsumer<ImageProcessingCommand>
    {
        private readonly IBus _bus;

        public ImageProcessingConsumer(IBus bus)
        {
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<ImageProcessingCommand> context)
        {
            string processedBase64 = ImageProcessingService.DarkenImage(context.Message.ImageData);

            if (processedBase64 != null)
            {
                await _bus.Publish(new ImageMergingCommand
                {
                    ImageData = processedBase64,
                    Part = context.Message.Part,
                    FileName = context.Message.FileName
                });
            }
        }
    }
}
