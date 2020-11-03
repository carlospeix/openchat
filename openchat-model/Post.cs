using System;
using System.Collections.Generic;

namespace OpenChat.Model
{
    public class Post
    {
        public Guid Id { get; }
        public String Text { get; }
        public DateTime PublicationTime { get; }
        public User Publisher { get; }

        public static Post Create(User publisher, string text, DateTime publicationTime)
        {
            return new Post(publisher, text, publicationTime);
        }

        private Post(User publisher, string text, DateTime publicationTime)
        {
            Id = Guid.NewGuid();
            Text = text;
            PublicationTime = publicationTime;
            Publisher = publisher;
        }
    }
}
