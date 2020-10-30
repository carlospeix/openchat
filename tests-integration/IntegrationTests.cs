using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenChat.Api;
using Xunit;

namespace OpenChat.Tests.Integration
{
    [Collection("Integration Tests")]
    public class IntegrationTests
    {
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetRoot_ReturnsSuccessAndStatusUp()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(
                await response.Content.ReadAsStringAsync());
            
            Assert.Equal("Up", (string)responseObject.status);
        }
    }
}
