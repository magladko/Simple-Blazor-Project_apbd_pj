using System;
using System.Collections.Generic;

namespace APBDproject.Shared.Models.DTOs
{
    public class ArticleDTO
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public DateTime PublishedUtc { get; set; }
        public string ArticleUrl { get; set; }
    }

    public class GetPolygonArticlesDTO
    {
        public IEnumerable<GetPolygonArticleDTO> Results;
    }

    public class GetPolygonArticleDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Published_utc { get; set; }
        public string Article_url { get; set; }
    }
}
