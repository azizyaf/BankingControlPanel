using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.DTOs.Clients;
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
    public class ClientsRepository : IClientsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClientsRepository> _logger;

        public ClientsRepository(ApplicationDbContext context, ILogger<ClientsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of clients based on the specified query parameters, including filtering and paging.
        /// </summary>
        /// <param name="parameters">The query parameters for filtering and paging.</param>
        /// <returns>A list of clients.</returns>
        public async Task<IEnumerable<Client>> GetClientsAsync(ClientsQueryParameters parameters)
        {
            try
            {
                IQueryable<Client> query = _context.Clients
                    .Include(c => c.Address)
                    .Include(c => c.Accounts);

                // Apply filters based on search term
                if (!string.IsNullOrEmpty(parameters.SearchTerm))
                {
                    query = query.Where(c => c.FirstName.Contains(parameters.SearchTerm) ||
                                             c.LastName.Contains(parameters.SearchTerm) ||
                                             c.Email.Contains(parameters.SearchTerm) ||
                                             c.PersonalId.Contains(parameters.SearchTerm) ||
                                             c.MobileNumber.Contains(parameters.SearchTerm) ||
                                             c.Address.City.Contains(parameters.SearchTerm) ||
                                             c.Address.Street.Contains(parameters.SearchTerm) ||
                                             c.Accounts.Any(a => a.AccountNumber.Contains(parameters.SearchTerm)));
                }

                // Filter by FirstName if provided
                if (!string.IsNullOrEmpty(parameters.FirstName))
                {
                    query = query.Where(c => c.FirstName.Contains(parameters.FirstName));
                }

                // Filter by LastName if provided
                if (!string.IsNullOrEmpty(parameters.LastName))
                {
                    query = query.Where(c => c.LastName.Contains(parameters.LastName));
                }

                // Filter by Email if provided
                if (!string.IsNullOrEmpty(parameters.Email))
                {
                    query = query.Where(c => c.Email.Contains(parameters.Email));
                }

                // Filter by PersonalId if provided
                if (!string.IsNullOrEmpty(parameters.PersonalId))
                {
                    query = query.Where(c => c.PersonalId == parameters.PersonalId);
                }

                // Filter by MobileNumber if provided
                if (!string.IsNullOrEmpty(parameters.MobileNumber))
                {
                    query = query.Where(c => c.MobileNumber.Contains(parameters.MobileNumber));
                }

                // Filter by Sex if provided
                if (parameters.Sex != default)
                {
                    query = query.Where(c => c.Sex == parameters.Sex);
                }

                // Apply Address filters
                if (!string.IsNullOrEmpty(parameters.Country))
                {
                    query = query.Where(c => c.Address.Country.Contains(parameters.Country));
                }

                if (!string.IsNullOrEmpty(parameters.City))
                {
                    query = query.Where(c => c.Address.City.Contains(parameters.City));
                }

                if (!string.IsNullOrEmpty(parameters.Street))
                {
                    query = query.Where(c => c.Address.Street.Contains(parameters.Street));
                }

                if (!string.IsNullOrEmpty(parameters.ZipCode))
                {
                    query = query.Where(c => c.Address.ZipCode.Contains(parameters.ZipCode));
                }

                // Apply Account filters
                if (!string.IsNullOrEmpty(parameters.AccountNumber))
                {
                    query = query.Where(c => c.Accounts.Any(a => a.AccountNumber.Contains(parameters.AccountNumber)));
                }

                if (!string.IsNullOrEmpty(parameters.AccountType))
                {
                    query = query.Where(c => c.Accounts.Any(a => a.AccountType.Contains(parameters.AccountType)));
                }

                if (parameters.MinBalance.HasValue)
                {
                    query = query.Where(c => c.Accounts.Any(a => a.Balance >= parameters.MinBalance.Value));
                }

                if (parameters.MaxBalance.HasValue)
                {
                    query = query.Where(c => c.Accounts.Any(a => a.Balance <= parameters.MaxBalance.Value));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(parameters.SortBy))
                {
                    query = parameters.SortBy.ToLower() switch
                    {
                        "firstname" => parameters.SortDescending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName),
                        "lastname" => parameters.SortDescending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),
                        "email" => parameters.SortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                        "personalid" => parameters.SortDescending ? query.OrderByDescending(c => c.PersonalId) : query.OrderBy(c => c.PersonalId),
                        "mobilenumber" => parameters.SortDescending ? query.OrderByDescending(c => c.MobileNumber) : query.OrderBy(c => c.MobileNumber),
                        _ => query // Default case if SortBy is not recognized
                    };
                }

                // Implement paging
                var skip = (parameters.PageNumber - 1) * parameters.PageSize;
                query = query.Skip(skip).Take(parameters.PageSize);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of clients.");
                throw new Exception("An error occurred while retrieving the list of clients.", ex);
            }
        }

        /// <summary>
        /// Retrieves the total number of clients based on the specified query parameters, including filtering.
        /// </summary>
        /// <param name="parameters">The query parameters for filtering.</param>
        /// <returns>The total number of clients that match the specified criteria.</returns>
        public async Task<int> GetTotalClientsAsync(ClientsQueryParameters parameters)
        {
            try
            {
                _logger.LogInformation("Retrieving total number of clients with given query parameters.");

                IQueryable<Client> query = _context.Clients.AsQueryable();

                // Apply filters based on search term
                if (!string.IsNullOrEmpty(parameters.SearchTerm))
                {
                    query = query.Where(c => c.FirstName.Contains(parameters.SearchTerm) ||
                                             c.LastName.Contains(parameters.SearchTerm) ||
                                             c.Email.Contains(parameters.SearchTerm) ||
                                             c.PersonalId.Contains(parameters.SearchTerm) ||
                                             c.MobileNumber.Contains(parameters.SearchTerm) ||
                                             c.Address.City.Contains(parameters.SearchTerm) ||
                                             c.Address.Street.Contains(parameters.SearchTerm) ||
                                             c.Accounts.Any(a => a.AccountNumber.Contains(parameters.SearchTerm)));
                }

                // Apply individual filters
                if (!string.IsNullOrEmpty(parameters.FirstName))
                {
                    query = query.Where(c => c.FirstName.Contains(parameters.FirstName));
                }

                if (!string.IsNullOrEmpty(parameters.LastName))
                {
                    query = query.Where(c => c.LastName.Contains(parameters.LastName));
                }

                if (!string.IsNullOrEmpty(parameters.Email))
                {
                    query = query.Where(c => c.Email.Contains(parameters.Email));
                }

                if (!string.IsNullOrEmpty(parameters.PersonalId))
                {
                    query = query.Where(c => c.PersonalId == parameters.PersonalId);
                }

                if (!string.IsNullOrEmpty(parameters.MobileNumber))
                {
                    query = query.Where(c => c.MobileNumber.Contains(parameters.MobileNumber));
                }

                if (parameters.Sex != default)
                {
                    query = query.Where(c => c.Sex == parameters.Sex);
                }

                // Apply Address filters
                if (!string.IsNullOrEmpty(parameters.Country))
                {
                    query = query.Where(c => c.Address.Country.Contains(parameters.Country));
                }

                if (!string.IsNullOrEmpty(parameters.City))
                {
                    query = query.Where(c => c.Address.City.Contains(parameters.City));
                }

                if (!string.IsNullOrEmpty(parameters.Street))
                {
                    query = query.Where(c => c.Address.Street.Contains(parameters.Street));
                }

                if (!string.IsNullOrEmpty(parameters.ZipCode))
                {
                    query = query.Where(c => c.Address.ZipCode.Contains(parameters.ZipCode));
                }

                // Apply Account filters
                if (!string.IsNullOrEmpty(parameters.AccountNumber))
                {
                    query = query.Where(c => c.Accounts.Any(a => a.AccountNumber.Contains(parameters.AccountNumber)));
                }

                if (!string.IsNullOrEmpty(parameters.AccountType))
                {
                    query = query.Where(c => c.Accounts.Any(a => a.AccountType.Contains(parameters.AccountType)));
                }

                if (parameters.MinBalance.HasValue)
                {
                    query = query.Where(c => c.Accounts.Any(a => a.Balance >= parameters.MinBalance.Value));
                }

                if (parameters.MaxBalance.HasValue)
                {
                    query = query.Where(c => c.Accounts.Any(a => a.Balance <= parameters.MaxBalance.Value));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the total number of clients.");
                throw new Exception("An error occurred while retrieving the total number of clients.", ex);
            }
        }

        /// <summary>
        /// Retrieves a client by its unique identifier.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <returns>The client if found; otherwise, null.</returns>
        public async Task<Client> GetClientByIdAsync(int clientId)
        {
            try
            {
                return await _context.Clients
                    .Include(c => c.Address) // Include the Address entity
                    .Include(c => c.Accounts) // Include the related Accounts
                    .FirstOrDefaultAsync(c => c.Id == clientId); // Use FirstOrDefaultAsync instead of FindAsync

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the client with ID {clientId}.");
                throw new Exception("An error occurred while retrieving the client by ID.", ex);
            }
        }

        /// <summary>
        /// Adds a new client to the database.
        /// </summary>
        /// <param name="client">The client to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddAsync(Client client)
        {
            try
            {
                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();
                _logger.LogInformation("A new client has been added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new client.");
                throw new Exception("An error occurred while adding a new client.", ex);
            }
        }

        /// <summary>
        /// Updates an existing client in the database.
        /// </summary>
        /// <param name="client">The client to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(Client client)
        {
            try
            {
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Client with ID {ClientId} has been updated successfully.", client.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the client with ID {client.Id}.");
                throw new Exception("An error occurred while updating the client.", ex);
            }
        }

        /// <summary>
        /// Deletes a client from the database.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(int clientId)
        {
            try
            {
                var client = await _context.Clients.FindAsync(clientId);
                if (client == null)
                {
                    _logger.LogWarning("Client with ID {ClientId} not found.", clientId);
                    throw new Exception("Client not found.");
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Client with ID {ClientId} has been deleted successfully.", clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the client with ID {clientId}.");
                throw new Exception("An error occurred while deleting the client.", ex);
            }
        }
    }
}
