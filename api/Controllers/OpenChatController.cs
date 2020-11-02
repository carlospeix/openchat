using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class OpenChatController : ControllerBase
    {
        private readonly RestDispatcher dispatcher;

        public OpenChatController(RestDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        [HttpGet("/openchat/")]
		public object GetStatus()
		{
			var responseObject = new
            {
				status = "Up"
			};

            return responseObject;
		}

        [HttpPost("/openchat/registration")]
        public IActionResult Registration([FromBody] RegistrationRequest request)
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