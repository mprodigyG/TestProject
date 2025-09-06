using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Web;
using System.IO;
using System.Linq;
//using System.Web.Mvc; // For MVC

namespace TestProject.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase {

        private readonly ILogger<TestController> _logger;
        private readonly string _settings;
        private DirectoryService _directoryService = new DirectoryService();

        public TestController(ILogger<TestController> logger, IOptions<ServerSettings> settings) {
            _logger = logger;
            _settings = settings.Value.HomeDirectoryPath;
        }

        [HttpGet("directoryInfo/{directoryPath}")]
        public IActionResult DirectoryInfo(string? directoryPath)
        {
            // use the directory path from the client supplies it, otherwise use value from settings
            string? useDirectory = string.IsNullOrEmpty(directoryPath) == true ? _settings : directoryPath;

            // directory does not exists inform the client
            if (_directoryService.CheckDirectory(directoryPath))
            {
                return NotFound();
            }

            return Ok(new
            {
                FileCount = _directoryService.GetFileCount(useDirectory)
                ,
                FolderCount = _directoryService.GetFolderCount(useDirectory)
                ,
                directoryPath = useDirectory
                ,
                DirectoryInfo = _directoryService.GetDirectoryInfo(useDirectory)
            });
        }

        [HttpPost("fileUpload/{directoryPath}")]
        public async Task<IActionResult> UploadSingleFile(string? directoryPath)
        {
            var uploadedFile = HttpContext.Request.Form.Files.GetFile("file");

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                // Define the path to save the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", uploadedFile.FileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                return Ok($"File '{uploadedFile.FileName}' uploaded successfully.");
            }

            return BadRequest("No file uploaded or file is empty.");
        }
    }
}