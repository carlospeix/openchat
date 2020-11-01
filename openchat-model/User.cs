using System;

namespace OpenChat.Model
{
    public class User
    {
        private readonly Guid id;
        private readonly string name;
        private readonly string about;

        public static User Create(string name, string about)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Can't create user with empty name.");

            return new User(name, about);
        }

        public User(string name, string about)
        {
            this.id = Guid.NewGuid();
            this.name = name;
            this.about = about;
        }

        public Guid Id => id;
        public string Name => name;
        public string About => about;
    }
}
