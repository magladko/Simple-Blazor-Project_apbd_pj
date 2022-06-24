using APBDproject.Server.Services;
using APBDproject.Shared.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APBDproject.Server.Controllers
{
    //[Authorize]
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
        [Route("{symbol}")]
        public async Task<bool> IsOnWatchlist(string symbol)
        {
            return await _watchlistService.IsOnWatchlistAsync(GetUserId(), symbol);
        }

        [HttpGet]
        public async Task<IEnumerable<CompanyDTO>> GetWatchedCompaniesAsync()
        {
            return await _watchlistService.GetWatchedCompaniesAync(GetUserId());
        }

        [HttpGet]
        [Route("all")]
        public async Task<WatchedCompaniesDTO> GetWatchedCompaniesToGridAsync()
        {
            var res = await GetWatchedCompaniesAsync();
            return new WatchedCompaniesDTO
            {
                Items = res,
                Count = res.Count()
            };
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string symbol)
        {
            try
            {
                await _watchlistService.AddCompanyToWatchlistAsync(GetUserId(), symbol);
                System.Diagnostics.Debug.WriteLine(GetUserId() + " AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
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
