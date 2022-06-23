using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBDproject.Shared.Models
{
    public class TickerSearchResponseModel
    {
        public IEnumerable<TickerModel> Results  { get; set; }
        public string Status { get; set; }
        
    }
}
