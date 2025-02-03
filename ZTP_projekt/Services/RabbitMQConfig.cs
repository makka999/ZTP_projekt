using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ZTP_projekt.Services
{
    public static class RabbitMQConfig
    {
        public static void ConfigureRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ImageProcessingConsumer>(); // Konsument przetwarzający obrazy
                x.AddConsumer<ImageMergingConsumer>();    // Konsument scalający obrazy

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("image_processing_queue", e =>
                    {
                        e.ConfigureConsumer<ImageProcessingConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("image_merge_queue", e =>
                    {
                        e.ConfigureConsumer<ImageMergingConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
