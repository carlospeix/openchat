using System;
using OpenChat.Model;

namespace OpenChat.Api
{
    public class RestDispatcher
    {
        public const int HTTP_CREATED = 201;

        private readonly OpenChatSystem system;

        public RestDispatcher()
        {
            system = new OpenChatSystem();
        }

        public DispatcherResponse RegisterUser(RegistrationRequest request)
        {
            var user = system.RegisterUser(request.username, request.password, request.about);

            var result = new UserResult(user);

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
}