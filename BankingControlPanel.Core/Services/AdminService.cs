using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.DTOs;
using BankingControlPanel.Core.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly ISearchParametersRepository _searchParametersRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AdminService> _logger;

        public AdminService(ISearchParametersRepository searchParametersRepository, IHttpContextAccessor httpContextAccessor, ILogger<AdminService> logger)
        {
            _searchParametersRepository = searchParametersRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Saves the search parameters to the database.
        /// </summary>
        /// <param name="queryParameters">The search parameters to save.</param>
        public async Task SaveSearchParametersAsync(ClientsQueryParameters queryParameters)
        {
            try
            {
                _logger.LogInformation("Saving search parameters.");

                // Retrieve the AdminId (UserId) from the bearer token
                var adminId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(adminId))
                {
                    throw new Exception("Unable to retrieve AdminId from the token.");
                }

                // Serialize ClientsQueryParameters to JSON for storage in the SearchCriteria field
                var serializedParameters = JsonSerializer.Serialize(queryParameters);

                var searchParameter = new SearchParameter
                {
                    SearchCriteria = serializedParameters,
                    SearchDate = DateTime.UtcNow,
                    AdminId = adminId // Set the retrieved AdminId
                };

                await _searchParametersRepository.AddSearchParameterAsync(searchParameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving search parameters.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the last 3 search parameters used by a specific admin.
        /// </summary>
        /// <returns>A list of the last 3 search parameters.</returns>
        public async Task<IEnumerable<ClientsQueryParameters>> GetLastSearchParametersAsync()
        {
            try
            {
                // Retrieve the AdminId (UserId) from the bearer token
                var adminId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(adminId))
                {
                    throw new Exception("Unable to retrieve AdminId from the token.");
                }

                _logger.LogInformation("Retrieving the last 3 search parameters for admin with ID {AdminId}.", adminId);

                var searchParameters = await _searchParametersRepository.GetLastSearchParametersAsync(adminId, 3);

                // Deserialize the SearchCriteria back into ClientsQueryParameters
                var deserializedParameters = new List<ClientsQueryParameters>();

                foreach (var parameter in searchParameters)
                {
                    var queryParameters = JsonSerializer.Deserialize<ClientsQueryParameters>(parameter.SearchCriteria);
                    deserializedParameters.Add(queryParameters);
                }

                return deserializedParameters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving search parameters.");
                throw;
            }
        }
    }
}
