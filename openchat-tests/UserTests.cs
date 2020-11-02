using OpenChat.Model;
using System;
using Xunit;

namespace OpenChat.Tests
{
    public class UserTests
    {
        private User carlos;

        public UserTests()
        {
            carlos = User.Create("Carlos", "About Carlos");
        }

        [Fact]
        public void CanCreateNewUser()
        {
            Assert.NotNull(carlos);
        }

        [Fact]
        public void NewUserHasProperNameAboutAndId()
        {
            Assert.NotEqual(Guid.Empty, carlos.Id);
            Assert.Equal("Carlos", carlos.Name);
            Assert.Equal("About Carlos", carlos.About);
        }

        [Fact]
        public void NewUserCreationFailsWithEmptyName()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => User.Create("", "")
            );

            Assert.Equal(User.MSG_CANT_CREATE_USER_WITH_EMPTY_NAME, exception.Message);
        }
    }
}
