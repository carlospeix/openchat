﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
            var response = dispatcher.RegisterUser(
                value.GetProperty("username").GetString(),
                value.GetProperty("password").GetString(),
                value.GetProperty("about").GetString());

            var result = new ObjectResult(response.Content)
            {
                StatusCode = response.Status
            };

            return result;
        }
    }
}