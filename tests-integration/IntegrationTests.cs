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
    public class IntegrationTests
    {
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing the piano and travelling." }
        // Success Status CREATED - 201 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing the piano and travelling." }
        // Failure Status: BAD_REQUEST - 400 Response: "Username already in use."
        public async Task User_RegisterNewUserSuccess()
        {
            // Arrange
            var alice = new { username = "Alice", password = "alki324d", about = "I love playing the piano and travelling." };
            var content = GetContentFrom(alice);

            // Act
            var httpResponse = await _client.PostAsync("/openchat/registration", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            dynamic response = await GetResponseFrom(httpResponse);
            Assert.NotEqual(Guid.Empty, Guid.Parse((string)response.userId));
            Assert.Equal(alice.username, (string)response.username);
            Assert.Equal(alice.about, (string)response.about);
        }

        [Fact]
        public async Task GetRoot_ReturnsSuccessAndStatusUp()
        {
            // Act
            var httpResponse = await _client.GetAsync("/openchat/");

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
    }
}
