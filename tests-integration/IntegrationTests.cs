using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenChat.Api;
using Xunit;

namespace OpenChat.Tests.Integration
{
    [Collection("Integration Tests")]
    public class IntegrationTests
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetRoot_ReturnsSuccessAndStatusUp()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Content);
            var statusObject = JsonSerializer.Deserialize<ResponseType>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            Assert.Equal("Up", statusObject.Status);
        }

        private class ResponseType
        {
            public string Status { get; set; }
        }
    }
}
