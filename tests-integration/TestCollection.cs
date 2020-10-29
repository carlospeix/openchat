using Microsoft.AspNetCore.Mvc.Testing;
using OpenChat.Api;
using Xunit;

namespace OpenChat.Tests.Integration
{
    [CollectionDefinition("Integration Tests")]
    public class TestCollection : ICollectionFixture<WebApplicationFactory<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}