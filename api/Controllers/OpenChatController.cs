using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class OpenChatController : ControllerBase
    {
        private readonly ILogger<OpenChatController> logger;

        public OpenChatController(ILogger<OpenChatController> logger)
        {
            this.logger = logger;
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
    }
}