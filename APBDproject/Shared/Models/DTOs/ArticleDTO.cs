using System;
using System.Collections;
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
        public IEnumerable<GetPolygonArticleDTO> results;
        //public string status { get; set; }
    }

    public class GetPolygonArticleDTO
    {
        public string id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public DateTime published_utc { get; set; }
        public string article_url { get; set; }
    }
}
