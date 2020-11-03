﻿using System;

namespace OpenChat.Model
{
    public class User
    {
        public const string MSG_CANT_CREATE_USER_WITH_EMPTY_NAME = "Can't create user with empty name.";

        public Guid Id { get; }
        public string Name { get; }
        public string About { get; }

        public static User Create(string name, string about = "")
        {
            AssertNameNotEmpty(name);

            return new User(name, about);
        }

        private static void AssertNameNotEmpty(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException(MSG_CANT_CREATE_USER_WITH_EMPTY_NAME);
        }

        private User(string name, string about)
        {
            Id = Guid.NewGuid();
            Name = name;
            About = about;
        }

        public Post Publish(string text, DateTime publicationTime)
        {
            return Post.Create(this, text, publicationTime);
        }

        public bool IdentifiedBy(Guid userId)
        {
            return Id.Equals(userId);
        }

        public bool Named(string userName)
        {
            return Name.Equals(userName);
        }

        public bool DescribedBy(string about)
        {
            return About.Equals(about);
        }
    }
}
