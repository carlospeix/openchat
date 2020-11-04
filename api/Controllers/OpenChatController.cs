using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OpenChat.Model;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class OpenChatController : ControllerBase
    {
        private readonly OpenChatSystem system;

        public OpenChatController(OpenChatSystem system)
        {
            this.system = system;
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
        public ObjectResult Registration([FromBody] RegistrationRequest request)
        {
            return system.RegisterUser<ObjectResult>(request.username, request.password, request.about,
                (user) => new CreatedResult($"/openchat/users/{user.Id}", new UserResult(user)),
                (message) => new BadRequestObjectResult(message));
        }

        [HttpPost("/openchat/login")]
        public ObjectResult Login([FromBody] LoginRequest request)
        {
            return system.LoginUser<ObjectResult>(request.username, request.password,
                (user) => new OkObjectResult(new UserResult(user)),
                (message) => new BadRequestObjectResult(message));
        }

        [HttpPost("/openchat/users/{userId}/posts")]
        public ObjectResult PublishPost([FromRoute] Guid userId, [FromBody] PublishPostRequest request)
        {
            return system.PublishPost<ObjectResult>(system.UserIdentifiedBy(userId), request.text,
                (post) => new CreatedResult($"/openchat/posts/{post.Id}", new PostResult(post)),
                (message) => new BadRequestObjectResult(message));
        }

        [HttpPost("/openchat/users/{userId}/timeline")]
        public ObjectResult UserTimeline([FromRoute] Guid userId)
        {
            return system.TimelineFor<ObjectResult>(system.UserIdentifiedBy(userId),
                (timeline) => new OkObjectResult(timeline.Select(post => new PostResult(post)).ToList()),
                (message) => new BadRequestObjectResult(message));
        }

        [HttpPost("/openchat/users/{followerId}/follow")]
        public IActionResult Follow([FromRoute] Guid followerId, [FromBody] FollowRequest request)
        {
            return system.Follow<IActionResult>(
                system.UserIdentifiedBy(followerId),
                system.UserIdentifiedBy(request.followeeId), 
                (follower) => new CreatedResult($"/openchat/users/{follower.Id}/followees", null));
        }
    }
    public class RegistrationRequest
    {
        public RegistrationRequest(string userName, string password, string about)
        {
            this.username = userName;
            this.password = password;
            this.about = about;
        }

        public string username;
        public string password;
        public string about;
    }

    public class UserResult
    {
        public UserResult(User user)
        {
            userId = user.Id;
            username = user.Name;
            about = user.About;
        }

        public Guid userId;
        public string username;
        public string about;
    }
    public class LoginRequest
    {
        public LoginRequest(string userName, string password)
        {
            this.username = userName;
            this.password = password;
        }

        public string username;
        public string password;
    }
    public class PublishPostRequest
    {
        public PublishPostRequest(string text)
        {
            this.text = text;
        }

        public string text;
    }
    public class PostResult
    {
        public PostResult(Post post)
        {
            postId = post.Id;
            userId = post.Publisher.Id;
            text = post.Text;
            publicationTime = post.PublicationTime;
        }

        public Guid postId;
        public Guid userId;
        public string text;
        public DateTime publicationTime;
    }
    public class FollowRequest
    {
        public FollowRequest(Guid followeeId)
        {
            this.followeeId = followeeId;
        }

        public Guid followeeId;
    }
}