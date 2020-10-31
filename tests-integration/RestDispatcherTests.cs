using System;
using Newtonsoft.Json;
using Xunit;
using OpenChat.Api;

namespace OpenChat.Tests.Integration
{
    public class RestDispatcherTests
    {
        [Fact]
        public void UserCreationSucceeds()
        {
            var dispatcher = new RestDispatcher();

            var request = new RegistrationRequest("Carlos", "Pass0rd!", "I like to sail in the open sea.");

            var response = dispatcher.RegisterUser(request);

            Assert.Equal(RestDispatcher.HTTP_CREATED, response.Status);

            var user = (UserResult)response.Content;

            Assert.NotEqual(Guid.Empty, (Guid)user.userId);
            Assert.Equal("Carlos", (string)user.username);
            Assert.Equal("I like to sail in the open sea.", (string)user.about);
        }

        private dynamic GetContentAsObject(DispatcherResponse response)
        {
            return JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(response.Content));
        }
    }
}