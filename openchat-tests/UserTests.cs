using OpenChat.Model;
using Xunit;

namespace OpenChat.Tests
{
    public class UserTests
    {
        [Fact]
        public void CanCreateNewUserWithNameAndAbout()
        {
            var newUser = User.Create("Carlos", "About Carlos");

            Assert.NotNull(newUser);
        }
    }
}
