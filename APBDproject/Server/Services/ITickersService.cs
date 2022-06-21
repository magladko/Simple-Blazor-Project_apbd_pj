using System.Collections;
using System.Threading.Tasks;

namespace APBDproject.Server.Services
{
    public interface ITickersService
    {
        public Task<IEnumerable> GetSearchTickers(string likeSymbol);
    }
}
