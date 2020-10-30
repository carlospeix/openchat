using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenChatController : ControllerBase
    {
        private readonly ILogger<OpenChatController> _logger;

        public OpenChatController(ILogger<OpenChatController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
		public object Get()
		{
			var responseObject = new
            {
				Status = "Up"
			};

			_logger.LogInformation($"Status pinged: {responseObject.Status}");
			
            return responseObject;
		}
    }
}