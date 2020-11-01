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

            Assert.Equal("Can't create user with empty name.", exception.Message);
        }
    }
}
