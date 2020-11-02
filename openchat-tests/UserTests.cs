using OpenChat.Model;
using System;
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

        [Fact]
        public void NewUserCreationFailsWithEmptyName()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => User.Create("", "")
            );

            Assert.Equal(User.MSG_CANT_CREATE_USER_WITH_EMPTY_NAME, exception.Message);
        }

        [Fact]
        public void NewUserHasProperNameAboutAndId()
        {
            var newUser = User.Create("Carlos", "About Carlos");

            Assert.NotEqual(Guid.Empty, newUser.Id);
            Assert.Equal("Carlos", newUser.Name);
            Assert.Equal("About Carlos", newUser.About);
        }

    }
}
