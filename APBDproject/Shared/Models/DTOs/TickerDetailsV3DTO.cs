using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBDproject.Shared.Models.DTOs
{
    public class TickerDetailsV3DTO
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Locale { get; set; }
        public string Sic_description { get; set; }
        public string Homepage_url { get; set; }
        public BrandingDTO branding { get; set; }
        

    }

    public class BrandingDTO
    {
        public string Logo_url { get; set; }
    }
}
