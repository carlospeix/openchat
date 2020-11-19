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

        public User RegisterUser(string userName, string password, string about)
        {
            AssertNewUserNameIsNotRegistered(userName);

            var credential = Credential.Create(password);
            var user = User.Create(userName, about);

            registeredUsers.Add(user);
            registeredCredentials.Add(user, credential);

            return user;
        }

        public User LoginUser(string userName, string password)
        {
            var user = registeredCredentials.SingleOrDefault(
                (kvp) => kvp.Key.IsNamed(userName) && kvp.Value.WithPassword(password)).Key;

            if (user == default(User))
                throw new InvalidOperationException(MSG_INVALID_CREDENTIALS);

            return user;
        }

        public int RegisteredUsersCount()
        {
            return registeredUsers.Count();
        }

        private void AssertNewUserNameIsNotRegistered(string userName)
        {
            if (registeredUsers.Any(user => user.IsNamed(userName)))
                throw new InvalidOperationException(MSG_USER_NAME_ALREADY_IN_USE);
        }

        public User UserIdentifiedBy(Guid userId)
        {
            return registeredUsers.Where(user => user.IsIdentifiedBy(userId)).FirstOrDefault();
        }

        public Post PublishPost(User publisher, string text)
        {
            if (UserIsNotRegistered(publisher))
                throw new InvalidOperationException(MSG_USER_DOES_NOT_EXIST);

            return publisher.Publish(text, clock.Now);
        }

        public IList<Post> TimelineFor(User publisher)
        {
            if (UserIsNotRegistered(publisher))
                throw new InvalidOperationException(MSG_USER_DOES_NOT_EXIST);

            return publisher.Timeline();
        }

        public IList<Post> WallFor(User publisher)
        {
            if (UserIsNotRegistered(publisher))
                throw new InvalidOperationException(MSG_USER_DOES_NOT_EXIST);

            return publisher.Wall();
        }

        public IList<User> Users()
        {
            return registeredUsers.AsReadOnly();
        }

        public User Follow(User follower, User followee)
        {
            if (UserIsNotRegistered(follower) || UserIsNotRegistered(followee))
                throw new InvalidOperationException(MSG_FOLLOWER_OR_FOLLOWEE_DOES_NOT_EXIST);

            return follower.Follow(followee);
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