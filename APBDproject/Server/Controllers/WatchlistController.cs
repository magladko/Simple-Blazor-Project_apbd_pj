using APBDproject.Server.Services;
using APBDproject.Shared.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<IEnumerable<CompanyDTO>> GetWatchedCompaniesAsync()
        {
            return await _watchlistService.GetWatchedCompaniesAync(GetUserId());
        }
        
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string symbol)
        {
            try
            {
                await _watchlistService.AddCompanyToWatchlistAsync(GetUserId(), symbol);
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
        public async Task<IActionResult> DeleteAsync([FromBody] string symbol)
        {
            try
            {
                await _watchlistService.RemoveCompanyFromWatchlistAsync(GetUserId(), symbol);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private string GetUserId()
        {
            return _http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
