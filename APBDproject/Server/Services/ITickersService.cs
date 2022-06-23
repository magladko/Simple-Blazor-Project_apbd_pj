using APBDproject.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APBDproject.Server.Services
{
    public interface ITickersService
    {
        public Task<IEnumerable<TickerModel>> GetSearchTickers(string likeSymbol);
        public Task<IEnumerable<TickerModel>> GetSearchTickersFromDb(string likeSymbol);
        public Task<bool> PostAllNewSearchTickersDb(IEnumerable<TickerModel> tickers);
        public Task<IEnumerable<SingleOHLC>> GetStockInfoAsync(string symbol);
        public Task<IEnumerable<SingleOHLC>> GetStockInfoDbAsync(string symbol);
    }
}
