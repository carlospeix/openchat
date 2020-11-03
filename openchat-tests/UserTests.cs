using OpenChat.Model;
using System;
using System.Reflection;
using Xunit;

namespace OpenChat.Tests
{
    public class UserTests
    {
        private readonly User carlos;

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

        [Fact]
        public void UserCanPublishPosts()
        {
            var publicationTime = DateTime.Now;
            var post = carlos.Publish("Nice post.", publicationTime);

            Assert.NotEqual(Guid.Empty, post.Id);
            Assert.Equal("Nice post.", post.Text);
            Assert.Equal(publicationTime, post.PublicationTime);
        }

        [Fact]
        public void UserIsThePublisherOfHerPost()
        {
            var publicationTime = DateTime.Now;
            var post = carlos.Publish("Nice post.", publicationTime);

            Assert.Equal(carlos, post.Publisher);
        }

    }
}
