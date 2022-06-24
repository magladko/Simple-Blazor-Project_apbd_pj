using System.Collections.Generic;

namespace APBDproject.Shared.Models.DTOs
{
    public class WatchedCompaniesDTO
    {
        public IEnumerable<CompanyDTO> Items { get; set; }
        public int Count { get; set; }
    }
}
