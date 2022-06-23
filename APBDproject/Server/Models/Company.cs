using System.Collections.Generic;

namespace APBDproject.Server.Models
{
    public class Company
    {
        public Company()
        {
            Users = new HashSet<ApplicationUser>();
            Articles = new HashSet<Article>();
        }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Locale { get; set; }
        public string SicDescription { get; set; }
        public string LogoUrl { get; set; }
        public string HomepageUrl { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual Daily Daily { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
    }
}
