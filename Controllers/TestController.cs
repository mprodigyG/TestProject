using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

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
        public IActionResult DirectoryInfo(string directoryPath)
        {
            string? useDirectory = _settings;
            // use the directory path from the client supplies it, otherwise use value from settings
            if (!string.IsNullOrEmpty(directoryPath))
            {
                useDirectory = directoryPath;
            }
            useDirectory = _settings;
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
                DirectoryInfo = _directoryService.GetDirectoryInfo(useDirectory)
            });
        }
    }
}