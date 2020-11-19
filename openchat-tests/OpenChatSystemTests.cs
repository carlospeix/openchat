using Newtonsoft.Json;
using OpenChat.Model;
using System;
using Xunit;
using Xunit.Sdk;

namespace OpenChat.Tests
{
    public class OpenChatSystemTests
    {
        private readonly OpenChatSystem system;

        public OpenChatSystemTests()
        {
            system = new OpenChatSystem();
        }

        [Fact]
        public void User_EnsurePasswordIsNotPresentInUserData()
        {
            var user = system.RegisterUser("irrelevant", "Pass0rd!", "");

            var json = JsonConvert.SerializeObject(user);
            Assert.DoesNotContain("Pass0rd!", json);
        }

        [Fact]
        public void CanLoginWithGoodUserNameAndPassword()
        {
            _ = system.RegisterUser("Carlos", "Pass0rd!", "");

            var user = system.LoginUser("Carlos", "Pass0rd!");

            Assert.True(user.IsNamed("Carlos"));
        }

        [Fact]
        public void CanNotLoginWithWrongPassword()
        {
            _ = system.RegisterUser("Carlos", "Pass0rd!", "");

            var exception = Assert.Throws<InvalidOperationException>(
                () => system.LoginUser("Carlos", "WRONG"));

            Assert.Equal(OpenChatSystem.MSG_INVALID_CREDENTIALS, exception.Message);
        }

        // CanNotLoginWithWrongUserName
        [Fact]
        public void CanNotLoginWithWrongUserName()
        {
            _ = system.RegisterUser("Carlos", "Pass0rd!", "");

            var exception = Assert.Throws<InvalidOperationException>(
                () => system.LoginUser("WRONG", "Pass0rd!"));

            Assert.Equal(OpenChatSystem.MSG_INVALID_CREDENTIALS, exception.Message);
        }

        [Fact]
        public void User_CanNotRegisterUserWithEmptyName()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => system.RegisterUser("", "password", ""));

            Assert.Equal(User.MSG_CANT_CREATE_USER_WITH_EMPTY_NAME, exception.Message);
        }

        [Fact]
        public void User_CanNotRegisterUserWithEmptyPassword()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => system.RegisterUser("irrelevant", "", ""));

            Assert.Equal(Credential.MSG_CANT_CREATE_CREDENTIAL_WITH_EMPTY_PASSWORD, exception.Message);
        }

        [Fact]
        public void User_CanNotRegisterSameUserTwice()
        {
            _ = system.RegisterUser("Alice", "irrelevant", "");

            var exception = Assert.Throws<InvalidOperationException>(
                () => system.RegisterUser("Alice", "irrelevant", ""));

            Assert.Equal(OpenChatSystem.MSG_USER_NAME_ALREADY_IN_USE, exception.Message);
            Assert.Equal(1, system.RegisteredUsersCount());
        }

        [Fact]
        public void Follow_CanFollowUser()
        {
            var follower = system.RegisterUser("Alice", "irrelevant", "");
            var followee = system.RegisterUser("Marta", "irrelevant", "");

            _ = system.Follow(follower, followee);

            Assert.Equal(1, follower.FolloweesCount());
        }

        [Fact]
        public void Follow_CanNotFollowNonExistentFollowee()
        {
            var follower = system.RegisterUser("Alice", "irrelevant", "");
            var nonExistingFollowee = User.Create("No existis");

            var exception = Assert.Throws<InvalidOperationException>(
                () => _ = system.Follow(follower, nonExistingFollowee));

            Assert.Equal(OpenChatSystem.MSG_FOLLOWER_OR_FOLLOWEE_DOES_NOT_EXIST, exception.Message);
            Assert.Equal(0, follower.FolloweesCount());
        }

        [Fact]
        public void Follow_NonExistenFollowerCanNotFollow()
        {
            var nonExistingFollower = User.Create("No existis");
            var followee = system.RegisterUser("Alice", "irrelevant", "");

            var exception = Assert.Throws<InvalidOperationException>(
                () => _ = system.Follow(nonExistingFollower, followee));

            Assert.Equal(OpenChatSystem.MSG_FOLLOWER_OR_FOLLOWEE_DOES_NOT_EXIST, exception.Message);
            Assert.Equal(0, nonExistingFollower.FolloweesCount());
        }

        [Fact]
        public void Follow_FollowingTheSameUserTwiceDoesNotFail()
        {
            var follower = system.RegisterUser("Alice", "irrelevant", "");
            var followee = system.RegisterUser("Marta", "irrelevant", "");

            _ = system.Follow(follower, followee);
            _ = system.Follow(follower, followee);

            Assert.Equal(1, follower.FolloweesCount());
        }
    }
}