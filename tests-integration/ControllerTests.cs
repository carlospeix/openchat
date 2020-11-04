using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OpenChat.Api.Controllers;
using OpenChat.Model;
using Xunit;

namespace OpenChat.Tests.Integration
{
    public class ControllerTests
    {
        private readonly Clock clock;
        private readonly OpenChatController controller;
        private readonly RegistrationRequest aliceRegistrationRequest;
        private readonly RegistrationRequest martaRegistrationRequest;

        public ControllerTests()
        {
            clock = Clock.Fake;
            controller = new OpenChatController(new OpenChatSystem(clock));
            
            aliceRegistrationRequest = new RegistrationRequest("Alice", "alki324d", "I love playing the piano and travelling.");
            martaRegistrationRequest = new RegistrationRequest("Marta", "irrelevant", "irrelevant");
        }

        // Register New User
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing the piano and travelling." }
        // Success Status CREATED - 201 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing the piano and travelling." }
        // Failure Status: BAD_REQUEST - 400 Response: "Username already in use."
        [Fact]
        public void User_RegisterNewUserSucceeds()
        {
            // Act
            var result = controller.Registration(aliceRegistrationRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            var userResult = (UserResult)result.Value;
            Assert.NotEqual(Guid.Empty, userResult.userId);
            Assert.Equal(aliceRegistrationRequest.username, userResult.username);
            Assert.Equal(aliceRegistrationRequest.about, userResult.about);
        }
        [Fact]
        public void User_RegisterSameUserTwiceFails()
        {
            // Arrange
            _ = controller.Registration(aliceRegistrationRequest);

            // Act
            var result = controller.Registration(aliceRegistrationRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal("Username already in use.", result.Value);
        }

        // Login
        // POST - openchat/login { "username" : "Alice" "password" : "alki324d" }
        // Success Status OK - 200 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing the piano and travelling." }
        // Failure Status: BAD_REQUEST - 400 Response: "Invalid credentials."
        [Fact]
        public void User_LoginSucceeds()
        {
            // Arrange
            _ = controller.Registration(aliceRegistrationRequest);
            var login = new LoginRequest(aliceRegistrationRequest.username, aliceRegistrationRequest.password);

            // Act
            var result = controller.Login(login);

            // Assert
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            var userResult = (UserResult)result.Value;
            Assert.Equal(aliceRegistrationRequest.username, userResult.username);
            Assert.Equal(aliceRegistrationRequest.about, userResult.about);
        }
        [Fact]
        public void User_LoginFailure()
        {
            // Arrange
            _ = controller.Registration(aliceRegistrationRequest);
            var login = new LoginRequest(aliceRegistrationRequest.username, aliceRegistrationRequest.password + "-X");

            // Act
            var result = controller.Login(login);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal(OpenChatSystem.MSG_INVALID_CREDENTIALS, result.Value);
        }

        // Create Post
        // POST openchat/users/{userId}/posts { "text" : "Hello everyone. I'm Alice." }
        // Success Status CREATED - 201 { "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hello everyone. I'm Alice.", "date" : "10/01/2018", "time" : "11:30:00" }
        // Failure Status: BAD_REQUEST - 400 (in case user does not exist) Response: "User does not exit."
        [Fact]
        public void Posts_PublishSucceeds()
        {
            // Arrange
            var registrationResult = controller.Registration(aliceRegistrationRequest);
            var userResult = (UserResult)registrationResult.Value;
            var userId = userResult.userId;

            var request = new PublishPostRequest("Hello everyone. I'm Alice.");

            // Act
            var result = controller.PublishPost(userId, request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            var publishPostResult = (PostResult)result.Value;
            Assert.NotEqual(Guid.Empty, publishPostResult.postId);
            Assert.Equal(userId, publishPostResult.userId);
            Assert.Equal(request.text, publishPostResult.text);
            Assert.Equal(clock.Now, publishPostResult.publicationTime);
        }
        [Fact]
        public void Posts_PublishFails()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new PublishPostRequest("Hello everyone. I'm Alice.");

            // Act
            var result = controller.PublishPost(userId, request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal(OpenChatSystem.MSG_USER_DOES_NOT_EXIST, result.Value);
        }

        // Retrieve Posts (User timeline)
        // GET - openchat/users/{userId}/timeline [{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "11:30:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hello everyone. I'm Alice.", "date" : "10/01/2018", "time" : "09:00:00" }]
        // Success Status OK - 200
        // Failure Status: BAD_REQUEST - 400 (in case user does not exist) Response: "User does not exit."
        [Fact]
        public void Posts_TimelineSucceeds()
        {
            // Arrange
            var registrationResult = controller.Registration(aliceRegistrationRequest);
            var userResult = (UserResult)registrationResult.Value;
            var userId = userResult.userId;
            var date1stPost = new DateTime(2018, 10, 1, 9, 0, 0);
            var date2ndPost = new DateTime(2018, 10, 1, 11, 30, 0);

            clock.Set(date1stPost);
            _ = controller.PublishPost(userId, 
                new PublishPostRequest("Hello everyone. I'm Alice.")); // "10/01/2018 09:00:00"
            
            clock.Set(date2ndPost);
            _ = controller.PublishPost(userId, 
                new PublishPostRequest("Anything interesting happening tonight?")); // "10/01/2018 11:30:00"

            // Act
            var result = controller.UserTimeline(userId);

            // Assert
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            var timelineResult = (IList<PostResult>)result.Value;
            Assert.Equal(2, timelineResult.Count());

            var topPost = timelineResult[0];
            Assert.Equal(userId, topPost.userId);
            Assert.Equal("Anything interesting happening tonight?", topPost.text);
            Assert.Equal(date2ndPost, topPost.publicationTime);

            var bottomPost = timelineResult[1];
            Assert.Equal(userId, bottomPost.userId);
            Assert.Equal("Hello everyone. I'm Alice.", bottomPost.text);
            Assert.Equal(date1stPost, bottomPost.publicationTime);
        }
        [Fact]
        public void Posts_TimelineFails()
        {
            // Arrange
            var registrationResult = controller.Registration(aliceRegistrationRequest);
            var userResult = (UserResult)registrationResult.Value;
            var userId = userResult.userId;
            var date1stPost = new DateTime(2018, 10, 1, 9, 0, 0);

            clock.Set(date1stPost);
            _ = controller.PublishPost(userId,
                new PublishPostRequest("Hello everyone. I'm Alice.")); // "10/01/2018 09:00:00"

            // Act
            var result = controller.UserTimeline(Guid.NewGuid());

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal(OpenChatSystem.MSG_USER_DOES_NOT_EXIST, result.Value);
        }

        // Follow User
        // POST - openchat/users/{userId}/follow { followerId: Alice ID, followeeId: Bob ID }
        // Success Status OK - 201
        // Failure Status: BAD_REQUEST - 400 (in case one of the users doesn't exist) Response: "At least one of the users does not exit."
        [Fact]
        public void Follow_NonExistentUserFails()
        {
            // Arrange
            var registrationResult = controller.Registration(aliceRegistrationRequest);
            var aliceResult = (UserResult)registrationResult.Value;
            var followerId = aliceResult.userId;
            var nonRegisteredUserId = Guid.NewGuid();

            // Act
            var result = controller.Follow(followerId, 
                new FollowRequest(nonRegisteredUserId)) as BadRequestObjectResult;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal(OpenChatSystem.MSG_FOLLOWER_OR_FOLLOWEE_DOES_NOT_EXIST, result.Value);
        }

        // Retrieve Wall
        // GET - openchat/users/{userId}/wall [{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "BOB_IDxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Planning to eat something with Charlie. Wanna join us?", "date" : "10/01/2018", "time" : "13:25:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "ALICE_ID-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "11:30:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "BOB_IDxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "What's up everyone?", "date" : "10/01/2018", "time" : "11:20:50" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "CHARLIE_IDxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hi all. Charlie here.", "date" : "10/01/2018", "time" : "09:15:34" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "ALICE_ID-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "09:00:00" }]
        // Success Status OK - 200
        // Failure Status: BAD_REQUEST - 400 (in case user does not exist) Response: "User does not exist."




        // Retrieve All Users
        // GET - openchat/users [{ "userId" : "123e4567-e89b-12d3-a456-426655440000", "username" : "Alice", "about" : "I love playing the pianno and travel.", },{ "userId" : "093f2342-e89b-12d3-a456-426655440000", "username" : "Bob", "about" : "Writer and photographer. Passionate about food and languages." },{ "userId" : "316h3543-e89b-12d3-a456-426655440000", "username" : "Charlie", "about" : "I'm a basketball player, love cycling and meeting new people. " }]
        // Success Status OK - 200




        // Retrieve all users followed by another user (followees)
        // GET - openchat/users/{userId}/followees [{ "userId" : "123e4567-e89b-12d3-a456-426655440000", "username" : "Alice", "about" : "I love playing the pianno and travel.", },{ "userId" : "093f2342-e89b-12d3-a456-426655440000", "username" : "Bob", "about" : "Writer and photographer. Passionate about food and languages." },{ "userId" : "316h3543-e89b-12d3-a456-426655440000", "username" : "Charlie", "about" : "I'm a basketball player, love cycling and meeting new people. " }]
        // Success Status OK - 200
    }
}
