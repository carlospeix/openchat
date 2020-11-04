using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenChat.Api;
using Xunit;
using System.Net;
using System;
using System.Text;
using Xunit.Abstractions;

namespace OpenChat.Tests.Integration
{
    // Integration tests definition
    // https://github.com/sandromancuso/cleancoders_openchat/blob/master/APIs.md

    [Collection("Integration Tests")]
    public class IntegrationTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly HttpClient client;

        public IntegrationTests(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            this.client = factory.CreateClient();
            this.output = output;
        }

        [Fact]
        public async Task GetRoot_ReturnsSuccessAndStatusUp()
        {
            // Act
            var httpResponse = await client.GetAsync("/openchat/");

            // Assert
            httpResponse.EnsureSuccessStatusCode();
            dynamic response = await GetResponseFrom(httpResponse);
            Assert.Equal("Up", (string)response.status);
        }

        [Fact]
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing the piano and travelling." }
        // Success Status CREATED - 201 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Mark", "about" : "I love playing the piano and travelling." }
        public async Task User_RegisterNewUserSuccess()
        {
            // Arrange
            var alice = new { username = "Alice", password = "alki324d", about = "I love playing the piano and travelling." };
            var content = GetContentFrom(alice);

            // Act
            var httpResponse = await client.PostAsync("/openchat/registration", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            dynamic response = await GetResponseFrom(httpResponse);
            Assert.NotEqual(Guid.Empty, Guid.Parse((string)response.userId));
            Assert.Equal(alice.username, (string)response.username);
            Assert.Equal(alice.about, (string)response.about);
        }

        // Create Post
        // POST openchat/users/{userId}/posts { "text" : "Hello everyone. I'm Alice." }
        // Success Status CREATED - 201 { "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hello everyone. I'm Alice.", "date" : "10/01/2018", "time" : "11:30:00" }
        [Fact]
        public async Task Post_PublishSuccess()
        {
            // Arrange
            var carlos = new { username = "Carlos", password = "alki324d", about = "I love playing the piano and travelling." };
            var httpRegistrationResponse = await client.PostAsync("/openchat/registration", GetContentFrom(carlos));
            dynamic registrationResponse = await GetResponseFrom(httpRegistrationResponse);
            var userId = Guid.Parse((string)registrationResponse.userId);

            // Act
            var post = new { text = "Hello everyone. I'm Carlos." };
            var httpPublishResponse = await client.PostAsync($"/openchat/users/{userId}/posts", GetContentFrom(post));

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpPublishResponse.StatusCode);
            dynamic response = await GetResponseFrom(httpPublishResponse);
            Assert.NotEqual(Guid.Empty, Guid.Parse((string)response.postId));
            Assert.Equal(userId, Guid.Parse((string)response.userId));
            Assert.Equal(post.text, (string)response.text);
        }

        // Follow User
        // POST - openchat/users/{userId}/follow { followerId: Alice ID, followeeId: Bob ID }
        // Success Status OK - 201
        [Fact]
        public async Task Follow_ConnieFollowsMartaSuccess()
        {
            // Arrange
            dynamic connieRegistration = await GetResponseFrom(
                await client.PostAsync("/openchat/registration", GetContentFrom(
                    new { username = "Connie", password = "alki324d", about = "" })));
            var followerId = Guid.Parse((string)connieRegistration.userId);

            dynamic martaRegistration = await GetResponseFrom(
                await client.PostAsync("/openchat/registration", GetContentFrom(
                    new { username = "Marta", password = "alki324d", about = "" })));
            var followeeId = Guid.Parse((string)martaRegistration.userId);

            // Act
            var followRequest = new { followeeId = followeeId };
            var httpPublishResponse = await client.PostAsync($"/openchat/users/{followerId}/follow", GetContentFrom(followRequest));

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpPublishResponse.StatusCode);
        }

        private HttpContent GetContentFrom(object content)
        {
            return new StringContent(
                JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }

        private async Task<dynamic> GetResponseFrom(HttpResponseMessage httpResponse)
        {
            return JsonConvert.DeserializeObject<dynamic>(
                await httpResponse.Content.ReadAsStringAsync());
        }

        public void Dispose()
        {
        }
    }
}
