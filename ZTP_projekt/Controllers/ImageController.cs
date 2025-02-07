using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using ZTP_projekt.Services;

namespace ZTP_projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageProcessingProducer _producer;

        public ImageController(ImageProcessingProducer producer)
        {
            _producer = producer;
        }

        [HttpPost("process-folder")]
        public async Task<IActionResult> ProcessAllImages()
        {
            await _producer.SendImageProcessingRequests();
            return Ok("Wysłano wszystkie obrazy do przetworzenia.");
        }

        [HttpPost("process-single")]
        public async Task<IActionResult> ProcessSingleImage([FromForm] IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Brak pliku.");

            string uploadPath = Path.Combine("wwwroot", "ProcessedImages", imageFile.FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            await _producer.SendImageProcessingRequest(uploadPath);
            return Ok($"Plik {imageFile.FileName} został wysłany do przetwarzania.");
        }
    }
}
