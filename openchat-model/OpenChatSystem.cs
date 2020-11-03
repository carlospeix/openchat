﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenChat.Model
{
    public class OpenChatSystem
    {
        private readonly Clock clock;
        private readonly List<User> registeredUsers = new List<User>();
        private readonly Dictionary<User, Credential> registeredCredentials = new Dictionary<User, Credential>();

        public const string MSG_USER_NAME_ALREADY_IN_USE = "Username already in use.";
        public const string MSG_INVALID_CREDENTIALS = "Invalid credentials.";

        public OpenChatSystem() : this(Clock.System)
        {
        }

        public OpenChatSystem(Clock clock)
        {
            this.clock = clock;
        }

        public User RegisterUser(string userName, string password, string about = "")
        {
            AssertNewUserNameDoesntExist(userName);

            var credential = Credential.Create(password);
            var user = User.Create(userName, about);

            registeredUsers.Add(user);
            registeredCredentials.Add(user, credential);

            return user;
        }

        public T LoginUser<T>(string userName, string password, Func<User, T> success, Func<string, T> fail)
        {
            var user = registeredUsers.Find(user => user.Named(userName));

            if (user != null && CredentialMatches(user, password))
                return success(user);
            else
                return fail(MSG_INVALID_CREDENTIALS);
        }

        private bool CredentialMatches(User user, string password)
        {
            return registeredCredentials.Any((kv) => kv.Key.Equals(user) && kv.Value.WithPassword(password));
        }

        private void AssertNewUserNameDoesntExist(string userName)
        {
            if (registeredUsers.Any(user => user.Named(userName)))
                throw new InvalidOperationException(MSG_USER_NAME_ALREADY_IN_USE);
        }

        public User UserIdentifiedBy(Guid userId)
        {
            return registeredUsers.Find(user => user.IdentifiedBy(userId));
        }

        public Post PublishPost(User user, string text)
        {
            var post = user.Publish(text, clock.Now);

            return post;
        }
    }
}