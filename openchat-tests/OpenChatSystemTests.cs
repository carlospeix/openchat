using Newtonsoft.Json;
using OpenChat.Model;
using System;
using Xunit;

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
        public void EnsurePasswordIsNotPresentInUserData()
        {
            var user = system.RegisterUser("irrelevant", "Pass0rd!", "",
                (user) => user,
                (message) => default /* for the linter */);

            var json = JsonConvert.SerializeObject(user);
            Assert.DoesNotContain("Pass0rd!", json);
        }

        [Fact]
        public void CantRegisterUserWithEmptyName()
        {
            var returnedMessage = system.RegisterUser("", "password", "",
                (user) => default,
                (message) => message);

            Assert.Equal(User.MSG_CANT_CREATE_USER_WITH_EMPTY_NAME, returnedMessage);
        }

        [Fact]
        public void CantRegisterUserWithEmptyPassword()
        {
            var returnedMessage = system.RegisterUser("irrelevant", "", "",
                (user) => default,
                (message) => message);

            Assert.Equal(Credential.MSG_CANT_CREATE_CREDENTIAL_WITH_EMPTY_PASSWORD, returnedMessage);
        }

        [Fact]
        public void CantRegisterSameUserTwice()
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
        public void CanFollowUser()
        {
            var aliceUser = system.RegisterUser("Alice", "irrelevant", "",
                (user) => user,
                (message) => default);
            var martaUser = system.RegisterUser("Marta", "irrelevant", "",
                (user) => user,
                (message) => default);

            _ = system.Follow(aliceUser, martaUser,
                (user) => user, (message) => default);

            Assert.Equal(1, aliceUser.FolloweesCount());
        }

        [Fact]
        public void CantFollowNonExistenUser()
        {
            var aliceUser = system.RegisterUser("Alice", "irrelevant", "",
                (user) => user,
                (message) => default);
            var martaUser = system.RegisterUser("Marta", "irrelevant", "",
                (user) => user,
                (message) => default);
            var nonExistingUser = User.Create("No existis");

            _ = system.Follow(aliceUser, martaUser, (user) => user, (message) => default);
            _ = system.Follow(aliceUser, nonExistingUser, (user) => user, (message) => default);

            Assert.Equal(1, aliceUser.FolloweesCount());
        }

        [Fact]
        public void FollowingTheSameUserTwiceDoesNotFail()
        {
            var aliceUser = system.RegisterUser("Alice", "irrelevant", "",
                (user) => user,
                (message) => default);
            var martaUser = system.RegisterUser("Marta", "irrelevant", "",
                (user) => user,
                (message) => default);

            _ = system.Follow(aliceUser, martaUser, (user) => user, (message) => default);
            _ = system.Follow(aliceUser, martaUser, (user) => user, (message) => default);

            Assert.Equal(1, aliceUser.FolloweesCount());
        }

    }
}