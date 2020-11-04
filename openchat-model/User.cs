using System;
using System.Linq;
using System.Collections.Generic;

namespace OpenChat.Model
{
    public class User
    {
        public const string MSG_CANT_CREATE_USER_WITH_EMPTY_NAME = "Can't create user with empty name.";

        public Guid Id { get; }
        public string Name { get; }
        public string About { get; }
        
        private readonly IList<Post> publishedPosts;
        private readonly IList<User> followees;

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

            publishedPosts = new List<Post>();
            followees = new List<User>();
        }

        public Post Publish(string text, DateTime publicationTime)
        {
            var post = Post.Create(this, text, publicationTime);
            publishedPosts.Add(post);

            return post;
        }

        internal User Follows(User followee)
        {
            if (!followees.Contains(followee))
                followees.Add(followee);
            return this;
        }

        public bool IsIdentifiedBy(Guid userId)
        {
            return Id.Equals(userId);
        }

        public bool IsNamed(string userName)
        {
            return Name.Equals(userName);
        }

        public bool IsDescribedBy(string about)
        {
            return About.Equals(about);
        }

        internal IList<Post> Timeline()
        {
            return publishedPosts
                .OrderByDescending(post => post.PublicationTime)
                .ToList()
                .AsReadOnly();
        }

        internal IList<Post> Wall() => Timeline();

        public int FolloweesCount()
        {
            return followees.Count();
        }
    }
}
