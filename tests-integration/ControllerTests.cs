﻿using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using OpenChat.Api;
using OpenChat.Api.Controllers;
using Xunit;

namespace OpenChat.Tests.Integration
{
    public class ControllerTests
    {
        private readonly OpenChatController controller;
        private readonly RegistrationRequest aliceRegistrationRequest;

        public ControllerTests()
        {
            controller = new OpenChatController(new RestDispatcher());
            aliceRegistrationRequest = new RegistrationRequest("Alice", "alki324d", "I love playing the piano and travelling.");
        }

        // Register New User
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing the piano and travelling." }
        // Success Status CREATED - 201 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing the piano and travelling." }
        // Failure Status: BAD_REQUEST - 400 Response: "Username already in use."
        [Fact]
        public void User_RegisterNewUserSuccess()
        {
            // Act
            var result = controller.Registration(aliceRegistrationRequest) as ObjectResult;

            // Assert
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            var userResult = (UserResult)result.Value;
            Assert.NotEqual(Guid.Empty, userResult.userId);
            Assert.Equal(aliceRegistrationRequest.username, userResult.username);
            Assert.Equal(aliceRegistrationRequest.about, userResult.about);
        }
        [Fact]
        public void User_RegisterSameUserTwiceFailure()
        {
            // Arrange
            _ = controller.Registration(aliceRegistrationRequest) as ObjectResult;

            // Act
            var result = controller.Registration(aliceRegistrationRequest) as ObjectResult;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal("Username already in use.", result.Value);
        }

        // Login
        // POST - openchat/login { "username" : "Alice" "password" : "alki324d" }
        // Success Status OK - 200 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing the piano and travelling." }
        // Failure Status: BAD_REQUEST - 400 Response: "Invalid credentials."
        [Fact]
        public void User_LoginSuccess()
        {
            // Arrange
            _ = controller.Registration(aliceRegistrationRequest) as ObjectResult;
            var login = new LoginRequest(aliceRegistrationRequest.username, aliceRegistrationRequest.password);

            // Act
            var result = controller.Login(login) as ObjectResult;

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
            _ = controller.Registration(aliceRegistrationRequest) as ObjectResult;
            var login = new LoginRequest(aliceRegistrationRequest.username, aliceRegistrationRequest.password + "-X");

            // Act
            var result = controller.Login(login) as ObjectResult;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal("Invalid credentials.", result.Value);
        }

        // Create Post
        // POST openchat/user//posts { "text" : "Hello everyone. I'm Alice." } Success Status CREATED - 201 { "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hello everyone. I'm Alice.", "date" : "10/01/2018", "time" : "11:30:00" }
        // Failure Status: BAD_REQUEST - 400 (in case user does not exist) Response: "User does not exit."




        // Retrieve Posts (User timeline)
        // GET - openchat/user//timeline [{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "11:30:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hello everyone. I'm Alice.", "date" : "10/01/2018", "time" : "09:00:00" }]
        // Success Status OK - 200
        // Failure Status: BAD_REQUEST - 400 (in case user does not exist) Response: "User does not exit."




        // Follow User
        // POST - openchat/follow { followerId: Alice ID, followeeId: Bob ID }
        // Success Status OK - 201
        // Failure Status: BAD_REQUEST - 400 (in case one of the users doesn't exist) Response: "At least one of the users does not exit."




        // Retrieve Wall
        // GET - openchat/user//wall [{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "BOB_IDxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Planning to eat something with Charlie. Wanna join us?", "date" : "10/01/2018", "time" : "13:25:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "ALICE_ID-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "11:30:00" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "BOB_IDxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "What's up everyone?", "date" : "10/01/2018", "time" : "11:20:50" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "CHARLIE_IDxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Hi all. Charlie here.", "date" : "10/01/2018", "time" : "09:15:34" },{ "postId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "userId" : "ALICE_ID-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "text" : "Anything interesting happening tonight?", "date" : "10/01/2018", "time" : "09:00:00" }]
        // Success Status OK - 200
        // Failure Status: BAD_REQUEST - 400 (in case user does not exist) Response: "User does not exist."




        // Retrieve All Users
        // GET - openchat/users [{ "userId" : "123e4567-e89b-12d3-a456-426655440000", "username" : "Alice", "about" : "I love playing the pianno and travel.", },{ "userId" : "093f2342-e89b-12d3-a456-426655440000", "username" : "Bob", "about" : "Writer and photographer. Passionate about food and languages." },{ "userId" : "316h3543-e89b-12d3-a456-426655440000", "username" : "Charlie", "about" : "I'm a basketball player, love cycling and meeting new people. " }]
        // Success Status OK - 200




        // Retrieve all users followed by another user (followees)
        // GET - openchat/user/:userId/followees [{ "userId" : "123e4567-e89b-12d3-a456-426655440000", "username" : "Alice", "about" : "I love playing the pianno and travel.", },{ "userId" : "093f2342-e89b-12d3-a456-426655440000", "username" : "Bob", "about" : "Writer and photographer. Passionate about food and languages." },{ "userId" : "316h3543-e89b-12d3-a456-426655440000", "username" : "Charlie", "about" : "I'm a basketball player, love cycling and meeting new people. " }]
        // Success Status OK - 200
    }
}
