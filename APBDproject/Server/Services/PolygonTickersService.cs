using APBDproject.Server.Data;
using APBDproject.Shared;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace APBDproject.Server.Services
{
    public class PolygonTickersService : ITickersService
    {
        private readonly HttpClient http = new HttpClient();
        private readonly ApplicationDbContext _context;

        public PolygonTickersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable> GetSearchTickers(string likeSymbol)
        {
            //var response = await http.GetFromJsonAsync<Ticker>($"https://api.polygon.io/v3/reference/tickers?active=true&sort=ticker&order=asc&limit=100&search={likeSymbol}&apiKey=sIyt0ncaofyKgKHqqXHpYTSuVrxVyK_N");
            //var result = response.results;

            throw new System.NotImplementedException();
        }
    }
}
