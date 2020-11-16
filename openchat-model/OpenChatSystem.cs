using System;
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
        public const string MSG_USER_DOES_NOT_EXIST = "User does not exist.";
        public const string MSG_FOLLOWER_OR_FOLLOWEE_DOES_NOT_EXIST = "At least one of the users does not exit.";

        public OpenChatSystem() : this(Clock.System)
        {
        }

        public OpenChatSystem(Clock clock)
        {
            this.clock = clock;
        }

        public T RegisterUser<T>(string userName, string password, string about, Func<User, T> success, Func<string, T> fail)
        {
            try
            {
                AssertNewUserNameDoesNotExist(userName);

                var credential = Credential.Create(password);
                var user = User.Create(userName, about);

                registeredUsers.Add(user);
                registeredCredentials.Add(user, credential);

                return success(user);
            }
            catch (Exception e)
            {
                return fail(e.Message);
            }
        }

        public T LoginUser<T>(string userName, string password, Func<User, T> success, Func<string, T> fail)
        {
            if (registeredCredentials.Any(
                (kvp) => 
                    kvp.Key.IsNamed(userName) && 
                    kvp.Value.WithPassword(password)))
                return success(registeredUsers.Find(user => user.IsNamed(userName)));
            else
                return fail(MSG_INVALID_CREDENTIALS);
        }

        public int RegisteredUsersCount()
        {
            return registeredUsers.Count();
        }

        private void AssertNewUserNameDoesNotExist(string userName)
        {
            if (registeredUsers.Any(user => user.IsNamed(userName)))
                throw new InvalidOperationException(MSG_USER_NAME_ALREADY_IN_USE);
        }

        public User UserIdentifiedBy(Guid userId)
        {
            return registeredUsers.Where(user => user.IsIdentifiedBy(userId)).FirstOrDefault();
        }

        public T PublishPost<T>(User publisher, string text, Func<Post, T> success, Func<string, T> fail)
        {
            if (UserIsNotRegistered(publisher))
                return fail(MSG_USER_DOES_NOT_EXIST);

            return success(publisher.Publish(text, clock.Now));
        }

        public T TimelineFor<T>(User publisher, Func<IList<Post>, T> success, Func<string, T> fail)
        {
            if (UserIsNotRegistered(publisher))
                return fail(MSG_USER_DOES_NOT_EXIST);

            return success(publisher.Timeline());
        }

        public T WallFor<T>(User publisher, Func<IList<Post>, T> success, Func<string, T> fail)
        {
            if (UserIsNotRegistered(publisher))
                return fail(MSG_USER_DOES_NOT_EXIST);

            return success(publisher.Wall());
        }

        public IList<User> Users()
        {
            return registeredUsers.AsReadOnly();
        }

        public T Follow<T>(User follower, User followee, Func<User, T> success, Func<string, T> fail)
        {
            if (UserIsNotRegistered(follower) || UserIsNotRegistered(followee))
                return fail(MSG_FOLLOWER_OR_FOLLOWEE_DOES_NOT_EXIST);

            return success(follower.Follow(followee));
        }

        public T FolloweesFor<T>(User user, Func<IList<User>, T> success, Func<string, T> fail)
        {
            if (UserIsNotRegistered(user))
                return fail(MSG_USER_DOES_NOT_EXIST);

            return success(user.Followees());
        }

        private bool UserIsNotRegistered(User userToVerify)
        {
            return !registeredUsers.Any(user => user.Equals(userToVerify));
        }
    }
}