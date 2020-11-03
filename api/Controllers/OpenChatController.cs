using Microsoft.AspNetCore.Mvc;
using System;

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

        [HttpPost("/openchat/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = dispatcher.Login(request);

            var result =
                new ObjectResult(response.Content)
                {
                    StatusCode = response.Status
                };

            return result;
        }

        [HttpPost("/openchat/users/{userId}/posts")]
        public IActionResult PublishPost([FromRoute] Guid userId, [FromBody] PublishPostRequest request)
        {
            request.userId = userId;
            var response = dispatcher.PublishPost(request);

            var result =
                new ObjectResult(response.Content)
                {
                    StatusCode = response.Status
                };

            return result;
        }
    }
}