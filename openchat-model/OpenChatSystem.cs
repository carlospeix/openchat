using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenChat.Model
{
    public class OpenChatSystem
    {
        private readonly List<User> registeredUsers = new List<User>();

        public User RegisterUser(string userName, string password, string about = "")
        {
            if (registeredUsers.Any(user => user.Name.Equals(userName)))
                throw new InvalidOperationException("Username already in use.");
            
            var user = User.Create(userName, about);
            registeredUsers.Add(user);
            
            return user;
        }
    }
}