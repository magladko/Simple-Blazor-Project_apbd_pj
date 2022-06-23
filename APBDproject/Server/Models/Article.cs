using System;
using System.Collections.Generic;

namespace APBDproject.Server.Models
{
    public class Article
    {
        public Article()
        {
            Companies = new HashSet<Company>();
        }
        
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public DateTime PublishedUtc { get; set; }
        public string ArticleUrl { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
    }
}
