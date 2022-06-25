using APBDproject.Server.Services;
using APBDproject.Shared.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APBDproject.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistService _watchlistService;
        private readonly IHttpContextAccessor _http;
        public WatchlistController(IWatchlistService watchlistService, IHttpContextAccessor http)
        {
            _watchlistService = watchlistService;
            _http = http;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            return await GetWatchedCompaniesToGridAsync();
        }

        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await DeleteAsync(id);
        }

        [HttpGet]
        [Route("isonlist/{symbol}")]
        public async Task<bool> IsOnWatchlist(string symbol)
        {
            //var id = GetUserId() == null ? "03119793-458e-46bf-a3ae-4f531567e698" : GetUserId(); // TODO: DELETE!
            return await _watchlistService.IsOnWatchlistAsync(GetUserId(), symbol);
        }

        [HttpGet]
        [Route("all")]
        public async Task<object> GetWatchedCompaniesToGridAsync()
        {
            var data = await _watchlistService.GetWatchedCompaniesAync(GetUserId());

            var count = data.Count();
            var queryString = Request.Query;
            if (queryString.Keys.Contains("$inlinecount"))
            {
                StringValues Skip;
                StringValues Take;
                StringValues OrderBy;
                int skip = (queryString.TryGetValue("$skip", out Skip)) ? Convert.ToInt32(Skip[0]) : 0;
                int top = (queryString.TryGetValue("$top", out Take)) ? Convert.ToInt32(Take[0]) : count;
                string orderBy = (queryString.TryGetValue("$orderby", out OrderBy)) ? OrderBy[0] : "Symbol";


                if (orderBy.Split(" ").Length > 1)
                {
                    return new { Items = data.OrderByDescending(c => c.GetType().GetProperty(orderBy.Split(" ")[0]).GetValue(c)).Skip(skip).Take(top), Count = count };
                }

                return new { Items = data.OrderBy(c => c.GetType().GetProperty(orderBy).GetValue(c)).Skip(skip).Take(top), Count = count };
            }
            else
            {
                return data;
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] PostTickerDTO symbol)
        {
            try
            {
                //var id = GetUserId() == null ? "03119793-458e-46bf-a3ae-4f531567e698" : GetUserId();
                await _watchlistService.AddCompanyToWatchlistAsync(GetUserId(), symbol.Symbol);
                //System.Diagnostics.Debug.WriteLine(GetUserId() + " AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("delete/{symbol}")]
        public async Task<IActionResult> DeleteAsync(string symbol)
        {
            try
            {
                //var id = GetUserId() == null ? "03119793-458e-46bf-a3ae-4f531567e698" : GetUserId(); // TODO: DELETE!
                await _watchlistService.RemoveCompanyFromWatchlistAsync(GetUserId(), symbol);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private string GetUserId()
        {
            try
            {
                return _http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            catch(NullReferenceException)
            {
                return null;
            }
        }
    }
}
