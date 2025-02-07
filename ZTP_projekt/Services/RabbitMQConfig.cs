using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ZTP_projekt.Services
{
    public static class RabbitMQConfig
    {
        public static void ConfigureRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ImageProcessingConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("image_processing_queue", e =>
                    {
                        e.PrefetchCount = 1;
                        e.ConfigureConsumer<ImageProcessingConsumer>(context);
                    });

                    Console.WriteLine("[INFO] Połączono z RabbitMQ!");
                });
            });
        }
    }
}
