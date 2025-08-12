using Blog.API.Filters;
using Blog.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.ComponentModel.DataAnnotations;

namespace Blog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKey] // Bảo vệ toàn bộ controller bằng API Key
    [AllowAnonymous]
    public class UploadFileController : ControllerBase
    {
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        //[AllowAnonymous]
        public IActionResult UploadFile([FromForm] UploadFileRequest request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var relativePath = UploadFileHelper.UploadFile(file);
            if (string.IsNullOrEmpty(relativePath))
                return StatusCode(400, "Upload failed.");

            return Ok(new { path = relativePath });
        }

        [HttpGet("download")]
        public IActionResult DownloadFile([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return BadRequest("Path is required.");

            try
            {
                var data = UploadFileHelper.GetDownloadData(path);
                return File(data.FileBytes, "application/octet-stream", data.FileName);
            }
            catch
            {
                return NotFound("File not found.");
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteFile([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return BadRequest("Path is required.");

            UploadFileHelper.RemoveFile(path);
            return Ok("File deleted (if it existed).");
        }

        [HttpGet("view")]
        public IActionResult ViewFile([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return BadRequest("Path is required.");

            try
            {
                // Lấy đường dẫn vật lý
                var physicalPath = UploadFileHelper.GetPhysicalPath(path);

                if (!System.IO.File.Exists(physicalPath))
                    return NotFound("File not found.");

                // Xác định Content-Type tự động
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(physicalPath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                // Trả về file với disposition inline (hiển thị trực tiếp)
                var fileStream = System.IO.File.OpenRead(physicalPath);
                return new FileStreamResult(fileStream, contentType)
                {
                    EnableRangeProcessing = true // Hỗ trợ partial content cho video/audio
                };
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Error retrieving file");
            }
        }


    }



    public class UploadFileRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
