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

            var result = UserResult.From(user);

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
        public string username;
        public string password;
        public string about;

        public RegistrationRequest(string userName, string password, string about)
        {
            this.username = userName;
            this.password = password;
            this.about = about;
        }
    }
    public class UserResult
    {
        public Guid userId;
        public string username;
        public string about;

        public static UserResult From(User user)
        {
            return new UserResult()
            {
                userId = user.Id,
                username = user.Name,
                about = user.About
            };
        }
    }
}