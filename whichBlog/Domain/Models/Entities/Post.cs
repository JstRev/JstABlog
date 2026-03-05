using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Entities
{
    public class Post
    {
        public Guid Id
        {
            get;
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Title cannot be null or empty.");
                }
                _title = value;
            }
        }

        private string _content;
        public string Content 
        {
            get => _content;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Content cannot be null or empty.");
                }
                _content = value;
            }
        }

        public DateTime CreatedAt
        {
            get;
            private set;
        }

        public Post(Guid id, string title, string content, DateTime createdAt)
        {
            Id = id;
            Title = title;
            Content = content;
            CreatedAt = createdAt;
        }

        public Post(string title, string content) : this(Guid.NewGuid(), title, content, DateTime.Now)
        {
            
        }

        public void ReloadDate()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
