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
            // TODO: fetch from DB (?)

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

        public async Task<MassiveCompanyDTO> GetCompanyDetailsAndInfoAsync(string symbol, int articlesLimit)
        {
            var daily = await GetDailyAsync(symbol);
            var ticker = await GetTickerDetailsAsync(symbol);
            var articles = await GetArticlesAsync(symbol, articlesLimit);

            return new MassiveCompanyDTO
            {
                Symbol = symbol,
                Name = ticker?.Name,
                Locale = ticker?.Locale,
                SicDescription = ticker?.SicDescription,
                LogoUrl = ticker?.LogoUrl,
                HomepageUrl = ticker?.HomepageUrl,
                Daily = daily,
                Articles = articles
            };
        }

        public async Task<DailyDTO> GetDailyAsync(string symbol)
        {
            DailyDTO result = null;
            try
            {
                result = await http.GetFromJsonAsync<DailyDTO>($"https://api.polygon.io/v1/open-close/{symbol}/{DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")}?adjusted=true&apiKey={_polygonApiKey}");
            }
            catch (Exception) { }

            if (result == null)
            {
                var resultDb = await GetDailyFromDbAsync(symbol);
                result = new DailyDTO
                {
                    From = resultDb.From,
                    Symbol = resultDb.Symbol,
                    Open = resultDb.Open,
                    High = resultDb.High,
                    Low = resultDb.Low,
                    Close = resultDb.Close,
                    Volume = resultDb.Volume,
                    AfterHours = resultDb.AfterHours,
                    PreMarket = resultDb.PreMarket
                };
            }


            if (!await _context.Daily.AnyAsync(d => d.Symbol == symbol && d.From == DateTime.Today.AddDays(-1)))
            {
                // TODO remove old Daily


                _context.Daily.Add(new Daily
                {
                    From = DateTime.Today.AddDays(-1),
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

        private async Task<Daily> GetDailyFromDbAsync(string symbol)
        {
            var result = await _context.Daily.Where(d => d.Symbol == symbol).OrderBy(d => d.From).FirstOrDefaultAsync();

            //if (result == null) throw new Exception($"No cached Daily data for {symbol}");

            return result;
        }

        public async Task<MassiveCompanyDTO> GetTickerDetailsAsync(string symbol)
        {
            MassiveCompanyDTO result;
            TickerDetailsV3DTO resultDTO = null;
            try
            {
                resultDTO = (await http.GetFromJsonAsync<TickerDetailsV3DTOWrapper>($"https://api.polygon.io/v3/reference/tickers/{symbol}?apiKey={_polygonApiKey}")).Results;
            }
            catch (Exception) { }

            if (resultDTO == null)
            {
                var resultDb = await GetTickerDetailsFromDbAsync(symbol);
                result = new MassiveCompanyDTO
                {
                    Symbol = resultDb?.Symbol,
                    Name = resultDb?.Name,
                    Locale = resultDb?.Locale,
                    SicDescription = resultDb?.SicDescription,
                    LogoUrl = resultDb?.LogoUrl,
                    HomepageUrl = resultDb?.HomepageUrl
                };
            }
            else
            {
                result = new MassiveCompanyDTO
                {
                    Symbol = resultDTO.Ticker,
                    Name = resultDTO.Name,
                    Locale = resultDTO.Locale,
                    SicDescription = resultDTO.Sic_description,
                    LogoUrl = resultDTO.Branding.Logo_url,
                    HomepageUrl = resultDTO.Homepage_url
                };

                result.LogoUrl += "?apiKey=" + (_polygonApiKey);

                await PostTickerDetailsChangesToDbIfNew(result);
            }

            return result;
        }

        private async Task<Company> GetTickerDetailsFromDbAsync(string symbol)
        {
            // takes data with null fields!
            var resultDb = await _context.Companies.Where(c => c.Symbol == symbol).SingleOrDefaultAsync();

            //if (resultDb == null) throw new Exception($"No such record with id {symbol}");

            return resultDb;
        }

        private async Task PostTickerDetailsChangesToDbIfNew(MassiveCompanyDTO company)
        {
            var companyDb = await _context.Companies.Where(c => c.Symbol == company.Symbol).SingleOrDefaultAsync();
            if (companyDb == null)
            {
                _context.Companies.Add(new Company
                {
                    Symbol = company.Symbol,
                    Name = company.Name,
                    Locale = company.Locale,
                    SicDescription = company.SicDescription,
                    LogoUrl = company.LogoUrl,
                    HomepageUrl = company.HomepageUrl,
                    LastUpdated = DateTime.Now
                });
            }
            else
            {
                _context.Companies.Update(companyDb);
                if (companyDb.Name != company.Name) companyDb.Name = company.Name;
                if (companyDb.Locale != company.Locale) companyDb.Locale = company.Locale;
                if (companyDb.SicDescription != company.SicDescription) companyDb.SicDescription = company.SicDescription;
                if (companyDb.LogoUrl != company.LogoUrl) companyDb.LogoUrl = company.LogoUrl;
                if (companyDb.HomepageUrl != company.HomepageUrl) companyDb.HomepageUrl = company.HomepageUrl;

                if (! (_context.Entry(companyDb).State == EntityState.Unchanged))
                {
                    companyDb.LastUpdated = DateTime.Now;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<ArticleDTO>> GetArticlesAsync(string symbol, int limit)
        {
            List<ArticleDTO> result = new List<ArticleDTO>();
            GetPolygonArticlesDTO resultDTO = null;
            try
            {
                resultDTO = JObject.Parse(await http.GetStringAsync($"https://api.polygon.io/v2/reference/news?ticker={symbol}&limit={limit}&apiKey={_polygonApiKey}")).ToObject<GetPolygonArticlesDTO>();
            }
            catch (Exception) { }

            if (resultDTO == null)
            {
                var resultDb = await GetArticlesFromDbAsync(symbol, limit);

                result.AddRange(
                    resultDb.Select(a => new ArticleDTO
                    {
                        Author = a?.Author,
                        Title = a?.Title,
                        PublishedUtc = a.PublishedUtc,
                        ArticleUrl = a?.ArticleUrl
                    }).ToList());
            }
            else
            {
                //foreach (var article in resultDTO.results)
                //{
                //    try
                //    {
                //        result.Add(new ArticleDTO
                //        {
                //            Author = article.author,
                //            Title = article.title,
                //            //PublishedUtc = DateTime.Parse(article.published_utc, null, System.Globalization.DateTimeStyles.RoundtripKind),
                //            PublishedUtc = article.published_utc,
                //            ArticleUrl = article.article_url
                //        });
                //    }
                //    catch(Exception e)
                //    {
                //        System.Diagnostics.Debug.WriteLine(e.Message);
                //    }
                //}

                result.AddRange(
                    resultDTO.results.Select(a => new ArticleDTO
                    {
                        Author = a.author,
                        Title = a.title,
                        PublishedUtc = a.published_utc,
                        ArticleUrl = a.article_url
                    }).ToList());

                foreach (var a in resultDTO.results)
                {
                    if (!await _context.Articles.AnyAsync(art => art.Id == a.id))
                    {
                        await _context.Articles.AddAsync(new Article
                        {
                            Id = a.id,
                            Author = a.author,
                            Title = a.title,
                            //PublishedUtc = DateTime.Parse(a.published_utc),
                            PublishedUtc = a.published_utc,
                            ArticleUrl = a.article_url
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            return result;
        }

        private async Task<IEnumerable<Article>> GetArticlesFromDbAsync(string symbol, int limit)
        {
            var company = await _context.Companies.Where(c => c.Symbol == symbol).SingleOrDefaultAsync();
            var result = await _context.Articles.Where(a => a.Companies.Contains(company)).OrderBy(a => a.PublishedUtc).Take(limit).ToListAsync();

            //if (result == null) throw new Exception($"No cached articles for {symbol}");

            return result;
        }
    }
}
