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
                var result = await http.GetFromJsonAsync<TickerWithOHLCs>($"https://api.polygon.io/v2/aggs/ticker/{symbol}/range/1/day/{DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd")}/{DateTime.Now.ToString("yyyy-MM-dd")}?adjusted=true&sort=asc&limit=200&apiKey=sIyt0ncaofyKgKHqqXHpYTSuVrxVyK_N");
                if (result == null) throw new Exception("No data received");

                return result.Results;
            }
            catch (Exception e)
            {
                var result = await GetStockInfoDbAsync(symbol);
                if (result == null) throw e;
                return result;
            }            
        }

        public async Task<IEnumerable<SingleOHLC>> GetStockInfoDbAsync(string symbol)
        {
            
            throw new NotImplementedException();
        }
    }
}
