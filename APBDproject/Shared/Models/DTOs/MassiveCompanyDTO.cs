using System.Collections.Generic;

namespace APBDproject.Shared.Models.DTOs
{
    public class MassiveCompanyDTO
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Locale { get; set; }
        public string SicDescription { get; set; }
        public string LogoUrl { get; set; }
        public string HomepageUrl { get; set; }
        public DailyDTO Daily { get; set; }
        public ICollection<ArticleDTO> Articles { get; set; }
    }
}
