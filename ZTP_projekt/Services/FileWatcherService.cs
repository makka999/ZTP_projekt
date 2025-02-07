using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZTP_projekt.Services
{
    public class FileWatcherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _inputFolder = Path.Combine("wwwroot", "ProcessedImages");

        public FileWatcherService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Directory.CreateDirectory(_inputFolder);

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = _inputFolder,
                Filter = "*.jpg",
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            watcher.Created += async (sender, args) =>
            {
                Console.WriteLine($"[INFO] Wykryto nowy plik: {args.FullPath}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var producer = scope.ServiceProvider.GetRequiredService<ImageProcessingProducer>();
                    await producer.SendImageProcessingRequest(args.FullPath);
                }
            };

            Console.WriteLine("[INFO] FileWatcher uruchomiony. Oczekuje na nowe pliki...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
