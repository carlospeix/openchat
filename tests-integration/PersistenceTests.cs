using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using OpenChat.Api;
using System.Net;

namespace OpenChat.Tests.Integration
{
    // Integration tests definition
    // https://github.com/sandromancuso/cleancoders_openchat/blob/master/APIs.md

    [Collection("Integration Tests")]
    public class PersistenceTests : IDisposable
    {
        private readonly ITestOutputHelper output;

        public PersistenceTests()
        {
        }

        [Fact(Skip = "Persistencia")]
        public async Task SystemCanPersistStateBetweenDifferentRuns()
        {
            using (var client = new WebApplicationFactory<Startup>().CreateClient())
            {
                var alice = new { username = "Alice", password = "alki324d", about = "" };
                _ = await client.PostAsync("/openchat/registration", GetJsonFrom(alice));
            };

            using (var client = new WebApplicationFactory<Startup>().CreateClient())
            {
                var httpUsersResponse = await client.GetAsync($"/openchat/users"); ;

                dynamic users = await GetResponseFromAsync(httpUsersResponse);
                Assert.Equal(1, users.Count);
                Assert.Equal("Alice", (string)users[0].username);
            };
        }

        private static HttpContent GetJsonFrom(object content)
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
