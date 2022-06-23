using System.Collections.Generic;

namespace APBDproject.Shared.Models
{
    public class TickerWithOHLCs
    {
        public string Ticker { get; set; }
        public IEnumerable<SingleOHLC> Results { get; set; }
    }
}
