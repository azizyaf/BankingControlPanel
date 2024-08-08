using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.Entities;
using BankingControlPanel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Infrastructure.Repositories
{
    public class SearchParametersRepository : ISearchParametersRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SearchParametersRepository> _logger;

        public SearchParametersRepository(ApplicationDbContext context, ILogger<SearchParametersRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new search parameter to the database.
        /// </summary>
        /// <param name="searchParameter">The search parameter to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddSearchParameterAsync(SearchParameter searchParameter)
        {
            try
            {
                await _context.SearchParameters.AddAsync(searchParameter);
                await _context.SaveChangesAsync();
                _logger.LogInformation("A new search parameter has been added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new search parameter.");
                throw new Exception("An error occurred while adding a new search parameter.", ex);
            }
        }

        /// <summary>
        /// Retrieves the last search parameters used by a specific admin.
        /// </summary>
        /// <param name="adminId">The unique identifier of the admin.</param>
        /// <param name="count">The number of search parameters to retrieve.</param>
        /// <returns>A list of search parameters.</returns>
        public async Task<List<SearchParameter>> GetLastSearchParametersAsync(string adminId, int count)
        {
            try
            {
                return await _context.SearchParameters
                    .Where(sp => sp.AdminId == adminId)
                    .OrderByDescending(sp => sp.SearchDate)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the last {count} search parameters for admin with ID {adminId}.");
                throw new Exception($"An error occurred while retrieving the last {count} search parameters.", ex);
            }
        }
    }
}
