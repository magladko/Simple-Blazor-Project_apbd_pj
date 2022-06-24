using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBDproject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistService _watchlistService;
        public WatchlistController(IWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_watchlistService.GetAll());
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_watchlistService.Get(id));
        }
        [HttpPost]
        public IActionResult Post(Watchlist watchlist)
        {
            _watchlistService.Add(watchlist);
            return Ok(watchlist);
        }
        [HttpPut("{id}")]
        public IActionResult Put(int id, Watchlist watchlist)
        {
            _watchlistService.Update(id, watchlist);
            return Ok(watchlist);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _watchlistService.Delete(id);
            return Ok();
        }
    }
}
