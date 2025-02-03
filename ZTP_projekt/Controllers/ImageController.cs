using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using ZTP_projekt.Services;
using ZTP_projekt.Models;

namespace ZTP_projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ImageProcessingProducer _producer; // Producent RabbitMQ

        public ImageController(IWebHostEnvironment env, ImageProcessingProducer producer)
        {
            _env = env;
            _producer = producer;
        }

        [HttpPost("darken")]
        public async Task<IActionResult> DarkenImage([FromForm] IFormFile imageFile, [FromServices] ImageProcessingProducer producer)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Brak pliku.");

            string fileName = $"{Guid.NewGuid()}.jpg";
            string savePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "ProcessedImages", fileName);

            // Tworzenie folderu, jeśli nie istnieje
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            // Zapis obrazu i natychmiastowe zamknięcie strumienia
            using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Wysłanie zadania do kolejki RabbitMQ
            await producer.SendImageProcessingRequest(savePath);

            return Ok(new { Message = "Obraz dodany do kolejki", FilePath = savePath });
        }
    }
}
