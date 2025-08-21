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
                return StatusCode(400, ApiResponse<string>.Fail("No file uploaded.", 400));

            var uploadResult = UploadFileHelper.UploadFile(file);
            return StatusCode(uploadResult.StatusCode, uploadResult);
        }

        [HttpGet("download")]
        public IActionResult DownloadFile([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return StatusCode(400, ApiResponse<string>.Fail("Path is required.", 400));

            var result = UploadFileHelper.GetDownloadData(path);
            if (!result.Success || result.Data == null)
            {
                return StatusCode(result.StatusCode, result);
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(result.Data.FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(result.Data.FileBytes, contentType, result.Data.FileName);
        }

        [HttpDelete("delete")]
        public IActionResult DeleteFile([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return StatusCode(400, ApiResponse<string>.Fail("Path is required.", 400));

            var result = UploadFileHelper.RemoveFile(path);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("view")]
        public IActionResult ViewFile([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return StatusCode(400, ApiResponse<string>.Fail("Path is required.", 400));

            try
            {
                var physicalPathResponse = UploadFileHelper.GetPhysicalPath(path);
                if (!physicalPathResponse.Success || string.IsNullOrWhiteSpace(physicalPathResponse.Data))
                {
                    return StatusCode(physicalPathResponse.StatusCode, physicalPathResponse);
                }

                var physicalPath = physicalPathResponse.Data;

                if (!System.IO.File.Exists(physicalPath))
                    return NotFound(ApiResponse<string>.Fail("File not found.", 404));

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(physicalPath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                var fileStream = System.IO.File.OpenRead(physicalPath);
                return new FileStreamResult(fileStream, contentType)
                {
                    EnableRangeProcessing = true
                };
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch
            {
                return StatusCode(500, ApiResponse<string>.Fail("Error retrieving file", 500));
            }
        }


    }



    public class UploadFileRequest
    {
        [Required]
        public required IFormFile File { get; set; }
    }
}
