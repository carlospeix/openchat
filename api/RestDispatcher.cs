using System;
using OpenChat.Model;

namespace OpenChat.Api
{
    public class RestDispatcher
    {
        public const int HTTP_OK = 200;
        public const int HTTP_CREATED = 201;
        public const int HTTP_BAD_REQUEST = 400;

        private readonly OpenChatSystem system;

        public RestDispatcher() : this(Clock.Real)
        {
        }

        public RestDispatcher(Clock clock)
        {
            system = new OpenChatSystem(clock);
        }

        public DispatcherResponse RegisterUser(RegistrationRequest request)
        {
            try
            {
                var user = system.RegisterUser(request.username, request.password, request.about);
                
                var result = new UserResult(user);
                return new DispatcherResponse(HTTP_CREATED, result);
            }
            catch (InvalidOperationException ex)
            {
                return new DispatcherResponse(HTTP_BAD_REQUEST, ex.Message);
            }
        }

        public DispatcherResponse Login(LoginRequest request)
        {
            return system.LoginUser(request.username, request.password,
                (user) => new DispatcherResponse(HTTP_OK, new UserResult(user)),
                (message) => new DispatcherResponse(HTTP_BAD_REQUEST, message));
       }

        internal DispatcherResponse PublishPost(PublishPostRequest request)
        {
            var user = system.UserIdentifiedBy(request.userId);
            if (user == null)
                return new DispatcherResponse(HTTP_BAD_REQUEST, "User does not exit.");

            var post = system.PublishPost(user, request.text);

            var result = new PublishPostResult(post);
            return new DispatcherResponse(HTTP_CREATED, result);
        }
    }

    public class DispatcherResponse
    {
        private readonly int status;
        private readonly object content;

        public DispatcherResponse(int status, object content)
        {
            this.status = status;
            this.content = content;
        }

        public int Status => status;
        public object Content => content;
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
        public PublishPostRequest(Guid userId, string text)
        {
            this.userId = userId;
            this.text = text;
        }

        public Guid userId;
        public string text;
    }
    public class PublishPostResult
    {
        public PublishPostResult(Post post)
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
}