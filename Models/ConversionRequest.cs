using Microsoft.AspNetCore.Http;

namespace ImageConverterAPI.Models
{
    public class ConversionRequest
    {
        public IFormFile File { get; set; }
        public string TargetFormat { get; set; }
        
        // Yeni eklenen alan: Kalite oranı (Örn: 20 ile 100 arasında)
        public int Quality { get; set; } = 100; // Varsayılan olarak 80 kalitede işlesin
    }
}