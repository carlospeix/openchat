using Newtonsoft.Json;
using OpenChat.Model;
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
    }
}