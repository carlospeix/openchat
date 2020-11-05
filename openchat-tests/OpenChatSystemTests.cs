using Newtonsoft.Json;
using OpenChat.Model;
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
            var user = system.RegisterUser("irrelevant", "Pass0rd!", "",
                (user) => user,
                (message) => default /* for the linter */);

            var json = JsonConvert.SerializeObject(user);
            Assert.DoesNotContain("Pass0rd!", json);
        }

        [Fact]
        public void CanLoginWithGoodUserNameAndPassword()
        {
            _ = system.RegisterUser<User>("Carlos", "Pass0rd!", "",
                (user) => default,
                (message) => default);

            var user = system.LoginUser<User>("Carlos", "Pass0rd!",
                (user) => user,
                (message) => throw new XunitException("Should had not failed becaus correct user name and password"));
        }

        [Fact]
        public void CanNotLoginWithWrongPassword()
        {
            _ = system.RegisterUser<User>("Carlos", "Pass0rd!", "",
                (user) => user,
                (message) => default);

            var user = system.LoginUser<User>("Carlos", "WRONG",
                (user) => throw new XunitException("Should had failed because of wrong password."),
                (message) => default);
        }

        // CanNotLoginWithWrongUserName
        [Fact]
        public void CanNotLoginWithWrongUserName()
        {
            _ = system.RegisterUser<User>("Carlos", "Pass0rd!", "",
                (user) => user,
                (message) => default);

            var user = system.LoginUser<User>("WRONG", "Pass0rd!",
                (user) => throw new XunitException("Should had failed because of wrong password."),
                (message) => default);
        }

        [Fact]
        public void User_CanNotRegisterUserWithEmptyName()
        {
            var returnedMessage = system.RegisterUser("", "password", "",
                (user) => default,
                (message) => message);

            Assert.Equal(User.MSG_CANT_CREATE_USER_WITH_EMPTY_NAME, returnedMessage);
        }

        [Fact]
        public void User_CanNotRegisterUserWithEmptyPassword()
        {
            var returnedMessage = system.RegisterUser("irrelevant", "", "",
                (user) => default,
                (message) => message);

            Assert.Equal(Credential.MSG_CANT_CREATE_CREDENTIAL_WITH_EMPTY_PASSWORD, returnedMessage);
        }

        [Fact]
        public void User_CanNotRegisterSameUserTwice()
        {
            _ = system.RegisterUser("Alice", "irrelevant", "",
                (user) => default,
                (message) => message);

            var returnedMessage = system.RegisterUser("Alice", "irrelevant", "",
                (user) => default,
                (message) => message);

            Assert.Equal(OpenChatSystem.MSG_USER_NAME_ALREADY_IN_USE, returnedMessage);
            Assert.Equal(1, system.RegisteredUsersCount());
        }

        [Fact]
        public void Follow_CanFollowUser()
        {
            var follower = system.RegisterUser("Alice", "irrelevant", "",
                (user) => user,
                (message) => default);
            var followee = system.RegisterUser("Marta", "irrelevant", "",
                (user) => user,
                (message) => default);

            _ = system.Follow(follower, followee,
                (user) => user, (message) => default);

            Assert.Equal(1, follower.FolloweesCount());
        }

        [Fact]
        public void Follow_CanNotFollowNonExistentFollowee()
        {
            var follower = system.RegisterUser("Alice", "irrelevant", "",
                (user) => user,
                (message) => default);
            var nonExistingFollowee = User.Create("No existis");

            _ = system.Follow<User>(follower, nonExistingFollowee,
                (user) => throw new XunitException("Should had failed because of non existent followee."),
                (message) => {
                    Assert.Equal(OpenChatSystem.MSG_FOLLOWER_OR_FOLLOWEE_DOES_NOT_EXIST, message);
                    return default;
                    });

            Assert.Equal(0, follower.FolloweesCount());
        }

        [Fact]
        public void Follow_NonExistenFollowerCanNotFollow()
        {
            var nonExistingFollower = User.Create("No existis");
            var followee = system.RegisterUser("Alice", "irrelevant", "",
                (user) => user,
                (message) => default);

            _ = system.Follow<User>(nonExistingFollower, followee, 
                (user) => throw new XunitException("Should had failed because of non existent followee."), 
                (message) => {
                    Assert.Equal(OpenChatSystem.MSG_FOLLOWER_OR_FOLLOWEE_DOES_NOT_EXIST, message);
                    return default;
                });

            Assert.Equal(0, nonExistingFollower.FolloweesCount());
        }

        [Fact]
        public void Follow_FollowingTheSameUserTwiceDoesNotFail()
        {
            var follower = system.RegisterUser("Alice", "irrelevant", "",
                (user) => user,
                (message) => default);
            var followee = system.RegisterUser("Marta", "irrelevant", "",
                (user) => user,
                (message) => default);

            _ = system.Follow(follower, followee, (user) => user, (message) => default);
            _ = system.Follow(follower, followee, (user) => user,
                (message) => throw new XunitException("Should had not failed because of duplicate follow."));

            Assert.Equal(1, follower.FolloweesCount());
        }
    }
}