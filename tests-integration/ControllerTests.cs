﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenChat.Api;
using OpenChat.Api.Controllers;
using Xunit;

namespace OpenChat.Tests.Integration
{
    public class ControllerTests
    {
        private readonly OpenChatController controller;

        public ControllerTests()
        {
            controller = new OpenChatController(new RestDispatcher());
        }

        [Fact]
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing the piano and travelling." }
        // Success Status CREATED - 201 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing the piano and travelling." }
        public void User_RegisterNewUserSuccess()
        {
            // Arrange
            var alice = new RegistrationRequest("Alice", "alki324d", "I love playing the piano and travelling.");

            // Act
            var result = controller.Registration(alice) as ObjectResult;

            // Assert
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            var userResult = (UserResult)result.Value;
            Assert.NotEqual(Guid.Empty, userResult.userId);
            Assert.Equal(alice.username, userResult.username);
            Assert.Equal(alice.about, userResult.about);
        }

        [Fact]
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing piano." }
        // Success Status CREATED - 201 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing piano." }
        // POST - openchat/registration { "username" : "Alice", "password" : "alki324d", "about" : "I love playing chess." }
        // Failure Status: BAD_REQUEST - 400 Response: "Username already in use."
        public void User_RegisterSameUserTwiceFails()
        {
            // Arrange
            var alice = new RegistrationRequest("Alice", "alki324d", "I love playing the piano and travelling.");
            _ = controller.Registration(alice) as ObjectResult;

            // Act
            var result = controller.Registration(alice) as ObjectResult;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal("Username already in use.", result.Value);
        }

        [Fact]
        // POST - openchat/login { "username" : "Alice" "password" : "alki324d" }
        // Success Status OK - 200 Response: { "userId" : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" "username" : "Alice", "about" : "I love playing the piano and travelling." }
        // Failure Status: BAD_REQUEST - 400 Response: "Invalid credentials."
        public void User_LoginSuccess()
        {
            // Arrange
            var alice = new RegistrationRequest("Alice", "alki324d", "I love playing the piano and travelling.");
            _ = controller.Registration(alice) as ObjectResult;
            var login = new LoginRequest(alice.username, alice.password);

            // Act
            var result = controller.Login(login) as ObjectResult;

            // Assert
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            var userResult = (UserResult)result.Value;
            Assert.Equal(alice.username, userResult.username);
            Assert.Equal(alice.about, userResult.about);
        }

        [Fact]
        // POST - openchat/login { "username" : "Alice" "password-X" : "alki324d" }
        // Failure Status: BAD_REQUEST - 400 Response: "Invalid credentials."
        public void User_LoginFail()
        {
            // Arrange
            var alice = new RegistrationRequest("Alice", "password", "I love playing the piano and travelling.");
            _ = controller.Registration(alice) as ObjectResult;
            var login = new LoginRequest(alice.username, alice.password + "-X");

            // Act
            var result = controller.Login(login) as ObjectResult;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal("Invalid credentials.", result.Value);
        }
    }
}
