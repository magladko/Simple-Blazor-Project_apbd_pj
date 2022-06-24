using APBDproject.Server.Data;
using APBDproject.Shared;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using APBDproject.Shared.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using APBDproject.Server.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using APBDproject.Shared.Models.DTOs;

namespace APBDproject.Server.Services
{
    public class PolygonTickersService : ITickersService
    {
        private readonly HttpClient http = new HttpClient();
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        private readonly string _polygonApiKey;

        public PolygonTickersService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
            _polygonApiKey = configuration.GetSection("PolygonApiKey").Value;
        }

        public async Task<IEnumerable<TickerModel>> GetSearchTickers(string likeSymbol)
        {
            TickerSearchResponseModel response;
            
            try
            {
                response = await http.GetFromJsonAsync<TickerSearchResponseModel>($"https://api.polygon.io/v3/reference/tickers?active=true&sort=ticker&order=asc&limit=0&search={likeSymbol}&apiKey={_polygonApiKey}"); //sIyt0ncaofyKgKHqqXHpYTSuVrxVyK_N
                if (response == null) throw new Exception();
                
                var result = response.Results;
                await PostAllNewSearchTickersDb(result);
                return response.Results;

            }
            catch (Exception)
            {
                return await GetSearchTickersFromDb(likeSymbol);
            }
        }

        public async Task<IEnumerable<TickerModel>> GetSearchTickersFromDb(string likeSymbol)
        {
            return await _context.Companies.Select(c => new TickerModel { Ticker = c.Symbol, Name = c.Name })
                .Where(t => t.Ticker.Contains(likeSymbol)).ToListAsync();
        }


        public async Task<bool> PostAllNewSearchTickersDb(IEnumerable<TickerModel> tickers)
        {
            foreach (var ticker in tickers)
            {
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.Symbol == ticker.Ticker);
                if (company == null)
                {
                    _context.Companies.Add(new Company
                    {
                        Symbol = ticker.Ticker,
                        Name = ticker.Name
                    });
                }
                if (company != null && company.Name != ticker.Name)
                {
                    _context.Companies.Where(c => c.Symbol == ticker.Ticker).SingleOrDefaultAsync().Result.Name = ticker.Name;
                }
            }
            
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SingleOHLC>> GetStockInfoAsync(string symbol)
        {
            // TODO: fetch from DB

            try
            {
                var result = await http.GetFromJsonAsync<TickerWithOHLCs>($"https://api.polygon.io/v2/aggs/ticker/{symbol}/range/1/day/{DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd")}/{DateTime.Now.ToString("yyyy-MM-dd")}?adjusted=true&sort=asc&limit=200&apiKey={_polygonApiKey}");
                if (result == null) throw new Exception("No data received");

                return result.Results;
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public async Task<MassiveCompanyDTO> GetCompanyDetailsAndInfoAsync(string symbol)
        {
            var daily = GetDailyAsync(symbol);
            var ticker = GetTickerDetailsAsync(symbol);
            var articles = GetArticlesAsync(symbol);

            throw new NotImplementedException();
        }

        public async Task<DailyDTO> GetDailyAsync(string symbol)
        {
            var result = await http.GetFromJsonAsync<DailyDTO>($"https://api.polygon.io/v1/open-close/{symbol}/{DateTime.Today.ToString("yyyy-MM-dd")}?adjusted=true&apiKey={_polygonApiKey}");

            if (result == null) result = await GetDailyFromDbAsync(symbol);

            if (!await _context.Daily.AnyAsync(d => d.From == DateTime.Today))
            {
                _context.Daily.Add(new Daily
                {
                    From = DateTime.Today,
                    Symbol = symbol,
                    Open = result.Open,
                    High = result.High,
                    Low = result.Low,
                    Close = result.Close,
                    Volume = result.Volume,
                    AfterHours = result.AfterHours,
                    PreMarket = result.PreMarket
                });
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<Daily> GetDailyFromDbAsync(string symbol)
        {
            var result = await _context.Daily.Where(d => d.Symbol == symbol).OrderBy(d => d.From).FirstOrDefaultAsync();

            if (result == null) throw new Exception("No data received");

            return result;
                
            //    new DailyDTO
            //{
            //    From = result.From,
            //    Symbol = result.Symbol,
            //    Open = result.Open,
            //    High = result.High,
            //    Low = result.Low,
            //    Close = result.Close,
            //    Volume = result.Volume,
            //    AfterHours = result.AfterHours,
            //    PreMarket = result.PreMarket
            //};
        }

        public async Task<MassiveCompanyDTO> GetTickerDetailsAsync(string symbol)
        {
            MassiveCompanyDTO result;

            var resultDTO = await http.GetFromJsonAsync<TickerDetailsV3DTO>($"https://api.polygon.io/v3/reference/tickers/{symbol}?apiKey={_polygonApiKey}");

            Company resultDb;
            if (resultDTO == null) resultDb = await GetTickerDetailsFromDbAsync(symbol);



            throw new NotImplementedException();
        }

        public async Task<Company> GetTickerDetailsFromDbAsync(string symbol)
        {
            var resultDb = await _context.Companies.Where(c => c.Symbol == symbol).SingleOrDefaultAsync();

            if (resultDb == null) throw new Exception($"No such record with id {symbol}");

            return resultDb;
        }

        public async Task<IEnumerable<ArticleDTO>> GetArticlesAsync(string symbol)
        {
            throw new NotImplementedException();
        }
    }
}
