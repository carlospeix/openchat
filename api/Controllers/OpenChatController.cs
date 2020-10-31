using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using OpenChat.Model;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class OpenChatController : ControllerBase
    {
        private readonly ILogger<OpenChatController> logger;
        private readonly RestDispatcher dispatcher;

        public OpenChatController(ILogger<OpenChatController> logger)
        {
            this.logger = logger;
            this.dispatcher = new RestDispatcher();
        }

        [HttpGet("/openchat/")]
		public object GetStatus()
		{
			var responseObject = new
            {
				status = "Up"
			};

			logger.LogInformation($"Status pinged: {responseObject.status}");
			
            return responseObject;
		}

        [HttpPost("/openchat/registration")]
        public IActionResult UserRegistration([FromBody] JsonElement value)
        {
            var userName = value.GetProperty("username").GetString();
            var password = value.GetProperty("password").GetString();
            var about = value.GetProperty("about").GetString();

            var response = dispatcher.RegisterUser(userName, password, about);

            var result = 
                new ObjectResult(response.Content)
                {
                    StatusCode = response.Status,
                };

            return result;
        }
    }
}