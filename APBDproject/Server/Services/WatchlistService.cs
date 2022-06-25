using APBDproject.Server.Data;
using APBDproject.Server.Models;
using APBDproject.Shared.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APBDproject.Server.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly ApplicationDbContext _context;

        public WatchlistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompanyDTO>> GetWatchedCompaniesAync(string userId)
        {
            var user = await _context.Users.Include(u => u.Companies).Where(user => user.Id == userId).SingleOrDefaultAsync();

            if (user == null) throw new KeyNotFoundException("User not found");

            //var order = orderBy.Split(" ")[0];
            //if (orderBy.Split(" ").Length > 1)
            //{
            //    return user.Companies.Select(c => new CompanyDTO
            //    {
            //        Symbol = c.Symbol,
            //        Name = c.Name,
            //        Locale = c.Locale,
            //        SicDescription = c.SicDescription
            //    }).OrderByDescending(c => c.GetType().GetProperty(order).GetValue(c)).Skip(skip).Take(top == -1 ? user.Companies.Count : top);
            //}

            return user.Companies.Select(c => new CompanyDTO
            {
                Symbol = c.Symbol,
                Name = c.Name,
                Locale = c.Locale,
                SicDescription = c.SicDescription
            }); // .OrderBy(c => c.GetType().GetProperty(order).GetValue(c)).Skip(skip).Take(top == -1 ? user.Companies.Count : top);
        }
        
        public async Task<bool> AddCompanyToWatchlistAsync(string userId, string symbol)
        {
            var user = await _context.Users.Include(u => u.Companies).Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (user == null) throw new KeyNotFoundException("User not found");


            var company = await _context.Companies.Where(c => c.Symbol == symbol).SingleOrDefaultAsync();
            if (company == null) throw new KeyNotFoundException("Company not found");

            user.Companies.Add(company);

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCompanyFromWatchlistAsync(string userId, string symbol)
        {
            var user = await _context.Users.Include(u => u.Companies).Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (user == null) throw new KeyNotFoundException("User not found");

            var company = user.Companies.Where(c => c.Symbol == symbol).SingleOrDefault();
            if (company == null) throw new KeyNotFoundException("Company not found");

            user.Companies.Remove(company);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsOnWatchlistAsync(string userId, string symbol)
        {
            var user = await _context.Users.Include(u => u.Companies).Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (user == null) return false;
            var company = user.Companies.Where(c => c.Symbol == symbol).SingleOrDefault();
            return company != null;
        }
    }
}
