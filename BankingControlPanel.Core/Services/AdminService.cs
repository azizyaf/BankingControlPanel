using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.DTOs.Clients;
using BankingControlPanel.Core.Models.Entities;
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
        private readonly ILogger<AdminService> _logger;
        private readonly IAuthService _authService;

        public AdminService(ISearchParametersRepository searchParametersRepository, ILogger<AdminService> logger, IAuthService authService)
        {
            _searchParametersRepository = searchParametersRepository;
            _logger = logger;
            _authService = authService;
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

                // Retrieve the current user (Admin) using the newly created method
                var currentUser = await _authService.GetCurrentUserAsync();

                if (currentUser == null)
                {
                    throw new Exception("Unable to retrieve the current user.");
                }

                // Serialize ClientsQueryParameters to JSON for storage in the SearchCriteria field
                var serializedParameters = JsonSerializer.Serialize(queryParameters);

                var searchParameter = new SearchParameter
                {
                    SearchCriteria = serializedParameters,
                    SearchDate = DateTime.UtcNow,
                    AdminId = currentUser.Id, // Set the retrieved AdminId
                    Admin = currentUser
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
                // Retrieve the current user (Admin) using the newly created method
                var currentUser = await _authService.GetCurrentUserAsync();

                if (currentUser == null)
                {
                    throw new Exception("Unable to retrieve the current user.");
                }

                _logger.LogInformation("Retrieving the last 3 search parameters for admin with ID {AdminId}.", currentUser.Id);

                var searchParameters = await _searchParametersRepository.GetLastSearchParametersAsync(currentUser.Id, 3);

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
