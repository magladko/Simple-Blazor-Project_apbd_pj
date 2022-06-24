using APBDproject.Shared.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APBDproject.Server.Services
{
    public interface IWatchlistService
    {
        public Task<IEnumerable<CompanyDTO>> GetWatchedCompaniesAync(string userId);
        public Task<bool> RemoveCompanyFromWatchlistAsync(string userId, string symbol);
        public Task<bool> AddCompanyToWatchlistAsync(string userId, string symbol);
        public Task<bool> IsOnWatchlistAsync(string userId, string symbol);
    }
}
