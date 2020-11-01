using System;

namespace OpenChat.Model
{
    public class User
    {
        public const string MSG_CANT_CREATE_USER_WITHEMPTY_NAME = "Can't create user with empty name.";

        public Guid Id { get; }
        public string Name { get; }
        public string About { get; }

        public static User Create(string name, string about)
        {
            AssertNameNotEmpty(name);

            return new User(name, about);
        }

        private static void AssertNameNotEmpty(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException(MSG_CANT_CREATE_USER_WITHEMPTY_NAME);
        }

        private User(string name, string about)
        {
            Id = Guid.NewGuid();
            Name = name;
            About = about;
        }
    }
}
