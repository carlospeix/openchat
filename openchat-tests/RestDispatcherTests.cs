using OpenChat.Model;
using System;
using Xunit;

namespace OpenChat.Tests
{
    public class RestDispatcherTests
    {
        [Fact]
        public void Test1()
        {
            var dispatcher = new RestDispatcher();

            var response = dispatcher.RegisterUser("", "", "");

            Assert.Equal(RestDispatcher.HTTP_CREATED, response.Status);
            //Assert.Equal("Alice", response.Content.username);
        }
    }
}
