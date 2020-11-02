using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenChat.Api;
using Xunit;
using System.Net;
using System;
using System.Text;

namespace OpenChat.Tests.Integration
{
    // Integration tests definition
    // https://github.com/sandromancuso/cleancoders_openchat/blob/master/APIs.md

    [Collection("Integration Tests")]
    public class IntegrationTests : IDisposable
    {
        private readonly HttpClient client;

        public IntegrationTests(WebApplicationFactory<Startup> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        // POST - openchat/registration { "username" : "Mark", "password" : "alki324d", "about" : "I love playing the piano and travelling." }
        // Success Status CREATED - 201 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Mark", "about" : "I love playing the piano and travelling." }
        public async Task User_RegisterNewUserSuccess()
        {
            // Arrange
            var alice = new { username = "Mark", password = "alki324d", about = "I love playing the piano and travelling." };
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

        [Fact]
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing piano." }
        // Success Status CREATED - 201 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing piano." }
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing chess." }
        // Failure Status: BAD_REQUEST - 400 Response: "Username already in use."
        public async Task User_RegisterSameUserTwiceFails()
        {
            var alicePiano = new { username = "Alice", password = "password1", about = "I love playing piano." };
            var aliceChess = new { username = "Alice", password = "password2", about = "I love playing chess." };
            _ = await client.PostAsync("/openchat/registration", GetContentFrom(alicePiano));
            
            var httpResponse = await client.PostAsync("/openchat/registration", GetContentFrom(aliceChess));

            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
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
