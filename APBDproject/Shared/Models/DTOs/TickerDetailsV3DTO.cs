namespace APBDproject.Shared.Models.DTOs
{
    public class TickerDetailsV3DTOWrapper
    {
        public TickerDetailsV3DTO Results { get; set; }
    }

    public class TickerDetailsV3DTO
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Locale { get; set; }
        public string Sic_description { get; set; }
        public string Homepage_url { get; set; }
        public BrandingDTO Branding { get; set; }
        

    }

    public class BrandingDTO
    {
        public string Logo_url { get; set; }
    }
}
