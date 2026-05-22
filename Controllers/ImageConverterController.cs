using Microsoft.AspNetCore.Mvc;
using ImageConverterAPI.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using System.IO.Compression;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace ImageConverterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageConverterController : ControllerBase
    {
        private const long MaxFileSize = 5 * 1024 * 1024; 
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        // ==========================================
        // METHOD 1: TEKLİ DOSYA DÖNÜŞTÜRME
        // ==========================================
        [HttpPost("convert")]
        public async Task<IActionResult> ConvertImage([FromForm] ConversionRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Lütfen geçerli bir dosya yükleyin.");

            var targetFormat = request.TargetFormat?.ToLower().Trim();
            
            if (targetFormat == "pdf")
            {
                return await ConvertMultipleImages(new List<IFormFile> { request.File }, "pdf", request.Quality);
            }

            if (request.File.Length > MaxFileSize)
                return BadRequest("Dosya boyutu 5 MB'tan büyük olamaz.");

            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                return BadRequest("Yalnızca JPG, JPEG, PNG ve WebP formatları desteklenmektedir.");

            int quality = request.Quality < 1 ? 1 : (request.Quality > 100 ? 100 : request.Quality);

            try
            {
                using var inputStream = request.File.OpenReadStream();
                // Sürüm 2.1.9 uyumlu senkron yükleme
                using var image = Image.Load(inputStream);
                
                var outputStream = new MemoryStream();
                string contentType = "";
                string newFileName = $"{Path.GetFileNameWithoutExtension(request.File.FileName)}.{targetFormat}";

                switch (targetFormat)
                {
                    case "jpeg":
                        image.SaveAsJpeg(outputStream, new JpegEncoder { Quality = quality });
                        contentType = "image/jpeg";
                        break;
                    case "png":
                        image.SaveAsPng(outputStream);
                        contentType = "image/png";
                        break;
                    case "webp":
                        image.SaveAsWebp(outputStream, new WebpEncoder { Quality = quality });
                        contentType = "image/webp";
                        break;
                }

                outputStream.Position = 0; 
                return File(outputStream, contentType, newFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Dosya işlenirken bir hata oluştu: {ex.Message}");
            }
        }

        // ==========================================
        // METHOD 2: TOPLU DOSYA DÖNÜŞTÜRME & PDF BİRLEŞTİRME
        // ==========================================
        [HttpPost("convert-multiple")]
        public async Task<IActionResult> ConvertMultipleImages([FromForm] List<IFormFile> files, [FromForm] string targetFormat, [FromForm] int quality)
        {
            if ((files == null || files.Count == 0) && Request.Form.Files.Count > 0)
            {
                files = Request.Form.Files.ToList();
            }

            if (files == null || files.Count == 0)
                return BadRequest("Lütfen en az bir geçerli dosya yükleyin.");

            var targetFormatLower = targetFormat?.ToLower().Trim();
            if (targetFormatLower != "jpeg" && targetFormatLower != "png" && targetFormatLower != "webp" && targetFormatLower != "pdf")
                return BadRequest("Geçersiz hedef format.");

            int targetQuality = quality < 1 ? 1 : (quality > 100 ? 100 : quality);

            try
            {
                // ------------- PDF DÖNÜŞÜMÜ -------------
                if (targetFormatLower == "pdf")
                {
                    var pdfStream = new MemoryStream();
                    using var document = new PdfDocument();

                    foreach (var file in files)
                    {
                        if (file.Length == 0) continue;

                        using var inputStream = file.OpenReadStream();
                        using var image = Image.Load(inputStream);
                        
                        var page = document.AddPage();
                        page.Width = image.Width;
                        page.Height = image.Height;

                        using var tempStream = new MemoryStream();
                        image.SaveAsJpeg(tempStream, new JpegEncoder { Quality = 95 });
                        var imageBytes = tempStream.ToArray();

                        using var imageMemoryStream = new MemoryStream(imageBytes);
                        using var xImage = XImage.FromStream(() => imageMemoryStream);
                        using var gfx = XGraphics.FromPdfPage(page);
                        
                        gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                    }

                    document.Save(pdfStream, false);
                    pdfStream.Position = 0;
                    return File(pdfStream, "application/pdf", "birlestirilen_dokuman.pdf");
                }

                // -------------  GÖRSEL FORMATLARI (ZIP) -------------
                var zipOutputStream = new MemoryStream();
                
                using (var archive = new ZipArchive(zipOutputStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        if (file.Length == 0) continue;

                        using var inputStream = file.OpenReadStream();
                        using var image = Image.Load(inputStream);
                        
                        using var imageOutputStream = new MemoryStream();

                        switch (targetFormatLower)
                        {
                            case "jpeg":
                                image.SaveAsJpeg(imageOutputStream, new JpegEncoder { Quality = targetQuality });
                                break;
                            case "png":
                                image.SaveAsPng(imageOutputStream);
                                break;
                            case "webp":
                                image.SaveAsWebp(imageOutputStream, new WebpEncoder { Quality = targetQuality });
                                break;
                        }

                        imageOutputStream.Position = 0;

                        var newFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}.{targetFormatLower}";
                        var zipEntry = archive.CreateEntry(newFileName);
                        
                        using var entryStream = zipEntry.Open();
                        await imageOutputStream.CopyToAsync(entryStream);
                    }
                }

                zipOutputStream.Position = 0;
                return File(zipOutputStream, "application/zip", "donusturulen_gorseller.zip");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"İşlem sırasında bir hata oluştu: {ex.Message}");
            }
        }
    }
}