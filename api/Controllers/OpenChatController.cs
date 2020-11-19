using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenChat.Model;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class OpenChatController : ControllerBase
    {
        private readonly OpenChatSystem system;
        private readonly ILogger<OpenChatController> logger;

        public OpenChatController(OpenChatSystem system, ILogger<OpenChatController> logger)
        {
            this.system = system;
            this.logger = logger;
        }

        [HttpGet("/openchat/")]
        public object GetStatus()
        {
            return new { status = "Up" };
        }

        [HttpPost("/openchat/registration")]
        public ObjectResult Registration([FromBody] RegistrationRequest request)
        {
            return DispatchRequest(
                () => system.RegisterUser(request.username, request.password, request.about),
                (user) => new CreatedResult($"/openchat/users/{user.Id}", new UserResult(user)));
        }

        [HttpPost("/openchat/login")]
        public ObjectResult Login([FromBody] LoginRequest request)
        {
            return DispatchRequest(
                () => system.LoginUser(request.username, request.password),
                (user) => new OkObjectResult(new UserResult(user)));
        }

        [HttpPost("/openchat/users/{userId}/posts")]
        public ObjectResult PublishPost([FromRoute] Guid userId, [FromBody] PublishPostRequest request)
        {
            return DispatchRequest(
                () => system.PublishPost(system.UserIdentifiedBy(userId), request.text),
                (post) => new CreatedResult($"/openchat/posts/{post.Id}", new PostResult(post)));
        }

        [HttpGet("/openchat/users/{userId}/timeline")]
        public ObjectResult UserTimeline([FromRoute] Guid userId)
        {
            return DispatchRequest(
                () => system.TimelineFor(system.UserIdentifiedBy(userId)),
                (timeline) => new OkObjectResult(timeline.Select(post => new PostResult(post)).ToList()));
        }

        [HttpPost("/openchat/users/{followerId}/follow")]
        public IActionResult Follow([FromRoute] Guid followerId, [FromBody] FollowRequest request)
        {
            return DispatchRequest(
                () => system.Follow(system.UserIdentifiedBy(followerId), system.UserIdentifiedBy(request.followeeId)),
                (follower) => new CreatedResult($"/openchat/users/{follower.Id}/followees", null));
        }

        [HttpGet("/openchat/users/{userId}/wall")]
        public ObjectResult UserWall([FromRoute] Guid userId)
        {
            return DispatchRequest(
                () => system.WallFor(system.UserIdentifiedBy(userId)),
                (wall) => new OkObjectResult(wall.Select(post => new PostResult(post)).ToList()));
        }

        [HttpGet("/openchat/users")]
        public ObjectResult Users()
        {
            return DispatchRequest(
                () => system.Users(),
                (users) => new OkObjectResult(users.Select(user => new UserResult(user)).ToList()));
        }

        [HttpGet("/openchat/users/{userId}/followees")]
        public ObjectResult Followees([FromRoute] Guid userId)
        {
            return DispatchRequest(
                () => system.FolloweesFor(system.UserIdentifiedBy(userId)),
                (followees) => new OkObjectResult(followees.Select(user => new UserResult(user)).ToList()));
        }

        public ObjectResult DispatchRequest<T>(Func<T> action, Func<T, ObjectResult> success)
        {
            try
            {
                return success(action.Invoke());
            }
            catch (InvalidOperationException ioe)
            {
                return new BadRequestObjectResult(ioe.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unhandled exception");
                return new ObjectResult("An internal server error has ocurred and is registered for further investigation.") { StatusCode = 500 };
            }
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