using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenChat.Model
{
    public class OpenChatSystem
    {
        private readonly List<User> registeredUsers = new List<User>();
        private readonly Dictionary<User, string> registeredCredentials = new Dictionary<User, string>();

        public const string MSG_USER_NAME_ALREADY_IN_USE = "Username already in use.";
        public const string MSG_INVALID_CREDENTIALS = "Invalid credentials.";
        public const string MSG_CANT_CREATE_CREDENTIAL_WITH_EMPTY_PASSWORD = "Can't create credential with empty password.";

        public User RegisterUser(string userName, string password, string about = "")
        {
            if (registeredUsers.Any(user => user.Name.Equals(userName)))
                throw new InvalidOperationException(MSG_USER_NAME_ALREADY_IN_USE);

            if (String.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(MSG_CANT_CREATE_CREDENTIAL_WITH_EMPTY_PASSWORD);


            var user = User.Create(userName, about);

            registeredUsers.Add(user);
            registeredCredentials.Add(user, password);
            
            return user;
        }

        public User LoginUser(string userName, string password)
        {
            var user = registeredUsers.Find(user => user.Name.Equals(userName));

            if (user == null || !registeredCredentials.Any((kv) => kv.Key.Equals(user) && kv.Value.Equals(password)))
                throw new InvalidOperationException(MSG_INVALID_CREDENTIALS);

            return user;
        }
    }
}