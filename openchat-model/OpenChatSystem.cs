using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenChat.Model
{
    public class OpenChatSystem
    {
        private readonly List<User> registeredUsers = new List<User>();
        private readonly Dictionary<User, string> registeredCredentials = new Dictionary<User, string>();

        public User RegisterUser(string userName, string password, string about = "")
        {
            if (registeredUsers.Any(user => user.Name.Equals(userName)))
                throw new InvalidOperationException("Username already in use.");
            
            var user = User.Create(userName, about);

            registeredUsers.Add(user);
            registeredCredentials.Add(user, password);
            
            return user;
        }

        public User LoginUser(string userName, string password)
        {
            var user = registeredUsers.Find(user => user.Name.Equals(userName));

            if (user == null || !registeredCredentials.Any((kv) => kv.Key.Equals(user) && kv.Value.Equals(password)))
                throw new InvalidOperationException("Invalid credentials.");

            return user;
        }
    }
}