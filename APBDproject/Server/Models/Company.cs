using System.Collections.Generic;

namespace APBDproject.Server.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Equity { get; set; }
        public string LogoUrl { get; set; }
        public string Country { get; set; }
        public string Industry { get; set; }
        public string CEO { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
