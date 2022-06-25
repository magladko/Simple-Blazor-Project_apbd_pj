using APBDproject.Server.Services;
using APBDproject.Shared.Models;
using APBDproject.Shared.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APBDproject.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TickersController : ControllerBase
    {
        private readonly ITickersService _service;

        public TickersController(ITickersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<TickerModel>> GetSearchedTickers([FromQuery(Name = "$filter")] string filter, 
            [FromQuery(Name = "$skip")] int skip, [FromQuery(Name = "$top")] int top)
        {
            string likeSymbol = filter.Split("(")[1].Split("'")[1];

            var result = await _service.GetSearchTickers(likeSymbol);

            return result;
        }

        [HttpGet]
        [Route("stock/{symbol}")]
        public async Task<IEnumerable<SingleOHLC>> GetStockInfo(string symbol)
        {
            try
            {
                var res = await _service.GetStockInfoAsync(symbol);

                return res;
            }
            catch (Exception)
            {
                //return null;
                throw;
            }
        }

        [HttpGet]
        [Route("details/{symbol}")]
        public async Task<MassiveCompanyDTO> GetCompanyDetailsAsync(string symbol, int articleLimit)
        {
            try
            { 
                return await _service.GetCompanyDetailsAndInfoAsync(symbol, articleLimit);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
