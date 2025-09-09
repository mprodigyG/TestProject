using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Web;
using System.IO;
using System.Linq;
//using System.Web.Mvc; // For MVC

namespace TestProject.Controllers {
    public class DirectoryLocation
    {
        public string sourceLocation { get; set; } = string.Empty;
        public string destinationLocation { get; set; } = string.Empty;
    }

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
            if (_directoryService.CheckDirectory(useDirectory) == false)
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
                ,
                DirectoryFolders = _directoryService.GetDirectoryFolders(useDirectory)
            });
        }

        [HttpGet("directoryInfo/{directoryPath}/{filterValue}")]
        public IActionResult DirectoryInfo(string? directoryPath, string filterValue)
        {
            // use the directory path from the client supplies it, otherwise use value from settings
            string? useDirectory = string.IsNullOrEmpty(directoryPath) == true ? _settings : directoryPath;

            // directory does not exists inform the client
            if (_directoryService.CheckDirectory(useDirectory) == false)
            {
                return NotFound();
            }

            return Ok(new
            {
                FileCount = _directoryService.GetFileCount(useDirectory, "*" + filterValue.Trim() + "*")
                ,
                FolderCount = _directoryService.GetFolderCount(useDirectory, "*" + filterValue.Trim() + "*")
                ,
                directoryPath = useDirectory
                ,
                DirectoryInfo = _directoryService.GetFilterDirectoryInfo(useDirectory, filterValue.Trim())
            });
        }

        [HttpGet("downloadfile/{directoryPath}")]
        public IActionResult DownloadFile(string directoryPath)
        {
            // Get the file name with its extension
            string fileName = Path.GetFileName(directoryPath);

            if (!System.IO.File.Exists(directoryPath))
            {
                return NotFound();
            }

            // Read the file into a byte array or stream
            var fileBytes = System.IO.File.ReadAllBytes(directoryPath);

            // Return the file as a FileContentResult with a download name
            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpPost("uploadfile/{directoryPath}")]
        public async Task<IActionResult> UploadFile(string directoryPath)
        {
            var uploadedFile = HttpContext.Request.Form.Files.GetFile("file");

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var filePath = Path.Combine(directoryPath, uploadedFile.FileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                return Ok($"File '{uploadedFile.FileName}' uploaded successfully.");
            }

            return BadRequest("No file uploaded or file is empty.");
        }

        [HttpDelete("deletefile/{directoryPath}")]
        public IActionResult DeleteFile(string directoryPath)
        {
            // the file from the requested location
            if (!System.IO.File.Exists(directoryPath))
            {
                return NotFound($"File '{Path.GetFileName(directoryPath)}' not found.");
            }

            try
            {
                System.IO.File.Delete(directoryPath);
                return Ok($"File '{Path.GetFileName(directoryPath)}' deleted successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("copyfile")]
        public IActionResult CopyFile([FromBody] DirectoryLocation directoryLocation)
        {
            // make a copy of the file within the same directory
            try
            {
                System.IO.File.Copy(directoryLocation.sourceLocation, directoryLocation.destinationLocation, true);

                return Ok("File copied successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpPost("movefile")]
        public IActionResult MoveFile([FromBody] DirectoryLocation directoryLocation)
        {
            // move the file from one directory to another one, this will remove the file from the current directory
            try
            {
                System.IO.File.Move(directoryLocation.sourceLocation, directoryLocation.destinationLocation);

                return Ok($"File moved from {directoryLocation.sourceLocation} to {directoryLocation.destinationLocation}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}