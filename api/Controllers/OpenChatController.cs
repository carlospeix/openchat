using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class OpenChatController : ControllerBase
    {
        private readonly ILogger<OpenChatController> logger;
        private readonly RestDispatcher dispatcher;

        public OpenChatController(ILogger<OpenChatController> logger, RestDispatcher dispatcher)
        {
            this.logger = logger;
            this.dispatcher = dispatcher;
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
        public IActionResult UserRegistration([FromBody] RegistrationRequest request)
        {
            var response = dispatcher.RegisterUser(request);

            var result = 
                new ObjectResult(response.Content)
                {
                    StatusCode = response.Status
                };

            return result;
        }
    }
}