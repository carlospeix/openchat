using System;

namespace OpenChat.Model
{
    public class RestDispatcher
    {
        public const int HTTP_CREATED = 201;

        public DispatcherResponse RegisterUser(string userName, string password, string about)
        {
            var user = new {
                userId = Guid.NewGuid().ToString(),
                username = userName,
                about = about
            };

            return new DispatcherResponse(HTTP_CREATED, user);
        }
    }

    public class DispatcherResponse
    {
        private int status;
        private object content;

        public DispatcherResponse(int status, object content)
        {
            this.status = status;
            this.content = content;
        }

        public int Status => status;
        public object Content => content;
    }
}