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
            AssertNewUserNameDoesntExist(userName);
            AsserPasswordNotEmpty(password);

            var user = User.Create(userName, about);

            registeredUsers.Add(user);
            registeredCredentials.Add(user, password);

            return user;
        }

        private void AssertNewUserNameDoesntExist(string userName)
        {
            if (registeredUsers.Any(user => user.Name.Equals(userName)))
                throw new InvalidOperationException(MSG_USER_NAME_ALREADY_IN_USE);
        }

        private static void AsserPasswordNotEmpty(string password)
        {
            if (String.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(MSG_CANT_CREATE_CREDENTIAL_WITH_EMPTY_PASSWORD);
        }

        public void LoginUser(string userName, string password, Action<User> success, Action<string> fail)
        {
            var user = registeredUsers.Find(user => user.Name.Equals(userName));

            if (user != null && registeredCredentials.Any((kv) => kv.Key.Equals(user) && kv.Value.Equals(password)))
                success(user);
            else
                fail(MSG_INVALID_CREDENTIALS);
        }
    }
}