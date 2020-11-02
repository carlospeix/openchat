using Newtonsoft.Json;
using OpenChat.Model;
using System;
using Xunit;

namespace OpenChat.Tests
{
    public class OpenChatSystemTests
    {

        [Fact]
        public void EnsurePasswordIsNotPresentInUserData()
        {
            var system = new OpenChatSystem();

            var user = system.RegisterUser("Carlos", "Pass0rd!");

            var json = JsonConvert.SerializeObject(user);
            Assert.DoesNotContain("Pass0rd!", json);
        }

        [Fact]
        public void EnsurePasswordIsNotEmpty()
        {
            var system = new OpenChatSystem();
            var exception = Assert.Throws<InvalidOperationException>(
                () => system.RegisterUser("Carlos", "")
            );

            Assert.Equal(OpenChatSystem.MSG_CANT_CREATE_CREDENTIAL_WITH_EMPTY_PASSWORD, exception.Message);
        }
    }
}