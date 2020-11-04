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
            dynamic response = await GetResponseFromAsync(httpResponse);
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
            dynamic response = await GetResponseFromAsync(httpResponse);
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
            var userId = await RegisterUserAsync("Carlos", "irrelevant", "");

            // Act
            var post = new { text = "Hello everyone. I'm Carlos." };
            var httpPublishResponse = await client.PostAsync($"/openchat/users/{userId}/posts", GetContentFrom(post));

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpPublishResponse.StatusCode);
            dynamic response = await GetResponseFromAsync(httpPublishResponse);
            Assert.NotEqual(Guid.Empty, Guid.Parse((string)response.postId));
            Assert.Equal(userId, Guid.Parse((string)response.userId));
            Assert.Equal(post.text, (string)response.text);
        }

        // Retrieve Posts (User timeline)
        // GET - openchat/users/{userId}/timeline [{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "11:30:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hello everyone. I'm Alice.", "date" : "10/01/2018", "time" : "09:00:00" }]
        // Success Status OK - 200
        // Failure Status: BAD_REQUEST - 400 (in case user does not exist) Response: "User does not exit."
        [Fact]
        public async Task Post_TimelineSuccess()
        {
            // Arrange
            var markId = await RegisterUserAsync("Mark", "irrelevant", "");
            var httpPublishResponse = await client.PostAsync($"/openchat/users/{markId}/posts",
                GetContentFrom(new { text = "Anything interesting happening tonight?" }));

            // Act
            var httpTimelineResponse = await client.GetAsync($"/openchat/users/{markId}/timeline");

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpTimelineResponse.StatusCode);
            dynamic response = await GetResponseFromAsync(httpTimelineResponse);
            Assert.Equal(1, response.Count);
        }

        // Follow User
        // POST - openchat/users/{userId}/follow { followerId: Alice ID, followeeId: Bob ID }
        // Success Status OK - 201
        [Fact]
        public async Task Follow_ConnieFollowsMartaSuccess()
        {
            // Arrange
            var followerId = await RegisterUserAsync("Connie", "irrelevant", "");
            var followeeId = await RegisterUserAsync("Marta", "irrelevant", "");

            // Act
            var followRequest = new { followeeId = followeeId };
            var httpPublishResponse = await client.PostAsync($"/openchat/users/{followerId}/follow", GetContentFrom(followRequest));

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpPublishResponse.StatusCode);
        }

        // Retrieve Wall
        // GET - openchat/users/{userId}/wall [{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "BOB_IDxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Planning to eat something with Charlie. Wanna join us?", "date" : "10/01/2018", "time" : "13:25:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "ALICE_ID-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "11:30:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "BOB_IDxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "What's up everyone?", "date" : "10/01/2018", "time" : "11:20:50" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "CHARLIE_IDxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hi all. Charlie here.", "date" : "10/01/2018", "time" : "09:15:34" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "ALICE_ID-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "09:00:00" }]
        // Success Status OK - 200
        // Failure Status: BAD_REQUEST - 400 (in case user does not exist) Response: "User does not exist."
        [Fact]
        public async Task Post_AliceWallSuccess()
        {
            // Arrange
            var aliceId = await RegisterUserAsync("Alice1", "irrelevant", "");
            _ = await client.PostAsync($"/openchat/users/{aliceId}/posts",
                GetContentFrom(new { text = "Anything interesting happening tonight?" }));

            // Act
            var httpWallResponse = await client.GetAsync($"/openchat/users/{aliceId}/wall");

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpWallResponse.StatusCode);
            dynamic response = await GetResponseFromAsync(httpWallResponse);
            Assert.Equal(1, response.Count);
        }

        private async Task<Guid> RegisterUserAsync(string userName, string password, string about)
        {
            var user = new { username = userName, password = password, about = about };
            var httpRegistrationResponse = await client.PostAsync("/openchat/registration", GetContentFrom(user));
            dynamic registrationResponse = await GetResponseFromAsync(httpRegistrationResponse);
            return Guid.Parse((string)registrationResponse.userId);
        }

        private HttpContent GetContentFrom(object content)
        {
            return new StringContent(
                JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }

        private async Task<dynamic> GetResponseFromAsync(HttpResponseMessage httpResponse)
        {
            return JsonConvert.DeserializeObject<dynamic>(
                await httpResponse.Content.ReadAsStringAsync());
        }

        public void Dispose()
        {
        }
    }
}
