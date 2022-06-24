using APBDproject.Server.Models;
using APBDproject.Shared.Models;
using APBDproject.Shared.Models.DTOs;
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
        public Task<MassiveCompanyDTO> GetCompanyDetailsAndInfoAsync(string symbol, int articlesLimit);
        public Task<DailyDTO> GetDailyAsync(string symbol);
        //public Task<Daily> GetDailyFromDbAsync(string symbol);
        public Task<MassiveCompanyDTO> GetTickerDetailsAsync(string symbol);
        //public Task<Company> GetTickerDetailsFromDbAsync(string symbol);
        public Task<ICollection<ArticleDTO>> GetArticlesAsync(string symbol, int limit);
        //public Task<IEnumerable<Article>> GetArticlesFromDbAsync(string symbol);
    }
}
